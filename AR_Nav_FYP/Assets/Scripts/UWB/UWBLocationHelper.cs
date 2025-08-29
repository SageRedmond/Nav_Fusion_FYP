using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Immersal.XR;
using System;
// using System.Numerics;

public class UWBLocationHelper : MonoBehaviour
{
  [SerializeField]
  private AnchorStorageManager anchorManager;

  [SerializeField]
  private XRSpace m_XRSpace;

  [SerializeField]
  private Dictionary<string, RoomZone> m_RoomZones = new Dictionary<string, RoomZone>();

  private static float MIN_HEIGHT_CONSTRAINT = 1.2f;
  private static float MAX_HEIGHT_CONSTRAINT = 0.7f;

  private Dictionary<int, XRMap> Maps = new Dictionary<int, XRMap>();

  [System.Serializable]
  public class Sphere
  {
    public float raduis;
    public Vector3 center;
  }

  public class SphereLocation
  {
    public float latitude;
    public float longitude;
  }

  // theta1 < Player Theta < theta2 
  class LongitudeConstraint
  {
    float maxTheta;
    float minTheta;

    public LongitudeConstraint(float maxTheta, float minTheta)
    {
      this.maxTheta = maxTheta;
      this.minTheta = minTheta;
    }
  }

  [SerializeField] public Sphere anchorSphere;

  private void Start()
  {
    GetMaps();
  }

  private void GetMaps()
  {
    XRMap[] allMaps = FindObjectsOfType<XRMap>();
    foreach (XRMap map in allMaps)
    {
      string mapName = string.Concat(map.mapName.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
      Maps.Add(map.mapId, map);
    }
  }

  private (float maxTheta, float minTheta) ComputeLongitudeConstraint(float raduis, Vector3 anchorCoords, float hMax, float hMin)
  {
    // float hMax = MAX_HEIGHT_CONSTRAINT;
    // float hMin = MIN_HEIGHT_CONSTRAINT;
    float anchorHeight = anchorCoords.y;

    float n = hMax + hMin - anchorHeight;
    float maxTheta = Mathf.Acos(n / raduis);

    float m = anchorHeight - hMin;
    float minTheta = 180.0f - Mathf.Acos(m / raduis);

    return (maxTheta, minTheta);
  }

  private (float maxPhi, float minPhi) ComputeLatitudeConstraints()
  {
    // Position must be within boundary of the room

    return (0.0f, 0.0f);
  }


  private IEnumerator GetLocalisationHint()
  {
    // 1. Get beacon XR location and range
    NativeState state = NativeStateManager.State;
    string id = state.beaconId;
    if (id == "")
    {
      Debug.LogError("No beacon connected");
      yield break;
    }

    float anchorDistance = state.distance;

    // 2. Check that there is a room tied to the beacon
    RoomZone roomZone = m_RoomZones[id];
    if (roomZone == null)
    {
      Debug.LogError("No room associated with beacon " + id);
      yield break;
    }

    Vector3? anchorPose = anchorManager.GetAnchorPoseByID(id);
    if (anchorPose == null)
    {
      Debug.LogError("No pose for anchor " + id);
      yield break;
    }
    // 3. Convert XR location to Unity Location
    Vector3 anchorPoseInUnitySpace = XRSpaceToUnity(m_XRSpace.transform, (Vector3)anchorPose);

    // 4. Compute Location Constraints
    (float maxTheta, float minTheta) = ComputeLongitudeConstraint(anchorDistance, anchorPoseInUnitySpace, MAX_HEIGHT_CONSTRAINT, MIN_HEIGHT_CONSTRAINT);
    Debug.Log("Max Theta " + maxTheta);
    Debug.Log("Min Theta " + minTheta);
    //TODO: Compute Latitude Constraint

    // 5. Pick a point on the sphere
    // Casting to integer because i want to and it's easier for Random()
    var rng = new System.Random();
    // 5.1 Get random longitude
    int randomLong = rng.Next((int)Math.Round(minTheta), (int)Math.Round(maxTheta));
    Debug.Log("Random Lat " + randomLong);

    Vector3 validPose = new Vector3(0, 0, 0);
    bool validPoseFound = false;
    while (!validPoseFound)
    {
      // 5.2 Get random latitude
      int randomLat = rng.Next(0, 361);
      Debug.Log("Random Long " + randomLat);

      // 5.3 Convert point to cartesion coords
      Vector3 randomPose = SphericalToCartesian(randomLat, randomLong, anchorPoseInUnitySpace, anchorDistance);

      // 5.4 Check if pose within room bounds
      validPoseFound = roomZone.CheckPoseInRoom(randomPose);
      if (validPoseFound)
      {
        validPose = randomPose;
      }
    }

    Debug.Log("Valid Pose Found at " + validPose);

    //TODO: Move Camera to that point
    // 6. Move camera to that pose (Convert to XR Space)
  }

  private Vector3 XRSpaceToUnity(Transform XRSpace, Matrix4x4 XRSpaceOffset, Vector3 pos)
  {
    Matrix4x4 m = XRSpace.worldToLocalMatrix;
    pos = m.MultiplyPoint(pos);
    pos = XRSpaceOffset.MultiplyPoint(pos);
    return pos;
  }

  private Vector3 XRSpaceToUnity(Transform XRSpace, Vector3 pos)
  {
    pos = XRSpaceToUnity(XRSpace, Matrix4x4.identity, pos);
    return pos;
  }

  public static Vector3 SphericalToCartesian(float latitude, float longitude, Vector3 sphereCenter, float radius)
  {
    float a = radius * Mathf.Cos(longitude);
    float xTemp = a * Mathf.Cos(latitude);
    float yTemp = radius * Mathf.Sin(longitude);
    float zTemp = a * Mathf.Sin(latitude);

    float x = xTemp + sphereCenter.x;
    float y = yTemp + sphereCenter.y;
    float z = zTemp + sphereCenter.z;

    return new Vector3(x, y, z);
  }

  // //! Doing it manually rn because can't be bothered with editing the rest API
  // private string GetRoomNameForAnchorID(string id)
  // {
  //   switch (id)
  //   {
  //     case "70f0576ae14090a92231974cccec402d":
  //       return "SittingRoom";
  //     default:
  //       return "";
  //   }
  // }
}
