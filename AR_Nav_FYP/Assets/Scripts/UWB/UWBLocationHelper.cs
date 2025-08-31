using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Immersal.XR;
using System;
using UnityEngine.XR.ARFoundation.VisualScripting;
using UnityEngine.UI;
using Unity.Mathematics;
// using System.Numerics;

public class UWBLocationHelper : MonoBehaviour
{
  [SerializeField]
  private AnchorStorageManager anchorManager;

  [SerializeField]
  private XRSpace m_XRSpace;

  [SerializeField]
  private GameObject cam;

  [SerializeField]
  private GameObject button;

  [SerializeField]
  private GameObject m_PositionPrefab;

  [SerializeField]
  private Dictionary<string, RoomZone> m_RoomZones = new Dictionary<string, RoomZone>();

  private static float MIN_HEIGHT_CONSTRAINT = 1.3f;
  private static float MAX_HEIGHT_CONSTRAINT = 0.7f;
  private static float MAX_RAYCAST_DISTANCE = 2;

  public LayerMask floorLayer;

  private Dictionary<int, XRMap> Maps = new Dictionary<int, XRMap>();

  private void Start()
  {
    GetMaps();
    GetRoomZones();
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

  private void GetRoomZones()
  {
    RoomZone[] rooms = FindObjectsOfType<RoomZone>();
    foreach (RoomZone room in rooms)
    {
      string anchorId = GetAnchorsIdForRoom(room.RoomName);
      m_RoomZones.Add(anchorId, room);
      print(room.RoomName);
    }
  }

  // Get a pose in XR space, then move the XR Space object so that the main camera is at that pose
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

    Vector3? anchorPoseXRSpace = anchorManager.GetAnchorPoseByID(id);
    if (anchorPoseXRSpace == null)
    {
      Debug.LogError("No pose for anchor " + id);
      yield break;
    }
    // 3. Convert XR location to Unity Location
    // Vector3 anchorPoseInUnitySpace = XRSpaceToUnity(m_XRSpace.transform, (Vector3)anchorPose);

    // 4. Compute Location Constraints
    (float maxTheta, float minTheta) = ComputeLongitudeConstraint((Vector3)anchorPoseXRSpace, anchorDistance, MAX_HEIGHT_CONSTRAINT, MIN_HEIGHT_CONSTRAINT);
    Debug.Log("Max Theta " + maxTheta);
    Debug.Log("Min Theta " + minTheta);
    //TODO: Compute Latitude Constraint (Phi)

    // 5. Pick a point on the sphere
    Vector3 validPoseXRSpace = ComputeValidPose((Vector3)anchorPoseXRSpace, anchorDistance, maxTheta, minTheta, roomZone);

    // 6. Move XR space so camera aligned with that pose
    //6.1 Get camera coords in XR Space cam.transform.localPosition
    Vector3 camXRPose = UnityToXRSpace(m_XRSpace.transform, m_XRSpace.InitialPose, cam.transform.localPosition);

    // 6.2 Create Translation Matrix Moving ValidPoseXRSpace to camXRPose
    Vector3 translation = new Vector3(camXRPose.x - validPoseXRSpace.x,
                                      camXRPose.y - validPoseXRSpace.y,
                                      camXRPose.z - validPoseXRSpace.z);
    Matrix4x4 transMatrix = Matrix4x4.Translate(translation);

    // 6.3 Calculate the new pose for the xr space and set
    Vector3 newXRSpacePose = transMatrix.MultiplyPoint3x4(m_XRSpace.transform.position);
    m_XRSpace.transform.SetPositionAndRotation(newXRSpacePose, Quaternion.identity);
  }

  private Vector3 ComputeValidPose(Vector3 anchorPoseXRSpace, float radius, float maxTheta, float minTheta, RoomZone roomZone)
  {
    Debug.LogError("Anchor Pose - " + anchorPoseXRSpace.ToString());
    // Casting to integer because i want to and it's easier for Random()
    var rng = new System.Random();
    // 5.1 Get random longitude
    int randomLong = rng.Next((int)Math.Round(minTheta), (int)Math.Round(maxTheta));
    // Vector3 testLat = SphericalToCartesian(randomLong, 0.0f, anchorPoseXRSpace, radius);
    // Vector3 testLong = SphericalToCartesian(0.0f, randomLong, anchorPoseXRSpace, radius);
    // //! Here
    // Debug.DrawLine(testLong, anchorPoseXRSpace, Color.white, 20.0f);
    // Debug.DrawLine(testLat, anchorPoseXRSpace, Color.red, 20.0f);
    Debug.Log("Random Long " + randomLong);

    Vector3 validPoseXRSpace = new Vector3(0, 0, 0);
    bool validPoseFound = false;
    int loopCount = 0;
    while (!validPoseFound)
    {
      loopCount += 1;
      // 5.2 Get random latitude
      int randomLat = rng.Next(0, 360);
      Debug.Log("Random Lat " + randomLat);

      // 5.3 Convert point to cartesion coords
      Vector3 randomPose = SphericalToCartesian(randomLat, randomLong, anchorPoseXRSpace, radius);

      // 5.4 Check if pose within room bounds
      validPoseFound = roomZone.CheckPoseInRoom(randomPose);
      if (validPoseFound)
      {
        validPoseXRSpace = randomPose;
      }
      if (loopCount >= 100)
      {
        Debug.LogError("To Many Loops");
        break;
      }
    }

    return validPoseXRSpace;
  }

  private (float maxTheta, float minTheta) ComputeLongitudeConstraint(Vector3 anchorCoords, float raduis, float hMax, float hMin)
  {
    // float hMax = MAX_HEIGHT_CONSTRAINT;
    // float hMin = MIN_HEIGHT_CONSTRAINT;
    // float anchorHeight = anchorCoords.y; // TODO: Change to raycasted height
    float anchorHeight = GetDistanceAnchorToFloor(anchorCoords);

    float m = anchorHeight - hMin;
    float maxTheta = 180.0f - (Mathf.Rad2Deg * Mathf.Acos(m / raduis));

    float n = (hMax + hMin) - anchorHeight;
    float minTheta = (Mathf.Rad2Deg * Mathf.Acos(n / raduis));

    return (maxTheta, minTheta);
  }

  private (float maxPhi, float minPhi) ComputeLatitudeConstraints()
  {
    // Position must be within boundary of the room

    return (0.0f, 0.0f);
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

  private Vector3 UnityToXRSpace(Transform XRSpace, Matrix4x4 XRSpaceOffset, Vector3 pos)
  {
    pos = XRSpaceOffset.inverse.MultiplyPoint(pos);
    Debug.LogAssertion(pos);
    Matrix4x4 m = XRSpace.localToWorldMatrix;
    pos = m.MultiplyPoint(pos);
    Debug.LogAssertion(pos);
    return pos;
  }

  private Vector3 UnityToXRSpace(Transform XRSpace, Vector3 pos)
  {
    pos = UnityToXRSpace(XRSpace, Matrix4x4.identity, pos);
    return pos;
  }

  public static Vector3 SphericalToCartesian(float latitude, float longitude, Vector3 sphereCenter, float radius)
  {
    // Made a mistake with my latitude calculation that needed to be corrected
    float a = radius * Mathf.Cos((longitude - 90.0f) * Mathf.Deg2Rad);
    float xTemp = a * Mathf.Cos(latitude * Mathf.Deg2Rad);
    float yTemp = radius * Mathf.Sin((longitude - 90.0f) * Mathf.Deg2Rad);
    float zTemp = a * Mathf.Sin(latitude * Mathf.Deg2Rad);

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

  private string GetAnchorsIdForRoom(string name)
  {
    switch (name)
    {
      case "Bedroom":
        return "70f0576ae14090a92231974cccec402d";
      default:
        return "";
    }
  }

  private float GetDistanceAnchorToFloor(Vector3 anchorPose)
  {
    RaycastHit hitData;
    Ray ray = new Ray(anchorPose, new Vector3(0.0f, -1.0f, 0.0f)); // Pointed down

    if (!Physics.Raycast(ray, out hitData, MAX_RAYCAST_DISTANCE, floorLayer))
    {
      Debug.Log("Didn't Hit Floor");
      return 0.0f;
    }
    float hitDistance = hitData.distance;
    Debug.Log("Hit Floor " + hitDistance);
    return hitDistance;
  }
  
  public void TestLocationHint(Transform testBeacon)
  {
    StartCoroutine(TestGettingHint(testBeacon));
  }
  public IEnumerator TestGettingHint(Transform testBeacon)
  {
    button.SetActive(false);
    yield return StartCoroutine(TestLocationHintCouroutine(testBeacon));
    button.SetActive(true);
  }

  public IEnumerator TestLocationHintCouroutine(Transform testBeacon)
  {
    Debug.Log("Starting Coroutine");
    float anchorDistance = 1.0f;

    // 2. Check that there is a room tied to the beacon
    RoomZone roomZone = m_RoomZones["70f0576ae14090a92231974cccec402d"];
    Vector3 anchorPoseXRSpace = testBeacon.position;
    // Vector3 anchorPoseXRSpace = UnityToXRSpace(m_XRSpace.transform, m_XRSpace.InitialPose, testBeacon.position);

    if (!roomZone.CheckPoseInRoom(anchorPoseXRSpace))
    {
      Debug.LogError("Anchor not inside room bounds");
      yield return null;
    }

    Debug.Log("ComputeLongitudeConstraint");
    (float maxTheta, float minTheta) = ComputeLongitudeConstraint(anchorPoseXRSpace, anchorDistance, MAX_HEIGHT_CONSTRAINT, MIN_HEIGHT_CONSTRAINT);
    Debug.Log("Max Theta " + maxTheta);
    Debug.Log("Min Theta " + minTheta);

    // 5. Pick a point on the sphere
    Vector3 validPoseXRSpace = ComputeValidPose(anchorPoseXRSpace, anchorDistance, maxTheta, minTheta, roomZone);
    Debug.LogWarning("Valid Pose" + validPoseXRSpace.ToString());
    GameObject go = Instantiate(m_PositionPrefab, validPoseXRSpace, Quaternion.identity, m_XRSpace.transform);
    // 6. Move XR space so camera aligned with that pose
    //6.1 Get camera coords in XR Space cam.transform.localPosition
    Vector3 camXRPose = UnityToXRSpace(m_XRSpace.transform, m_XRSpace.InitialPose, cam.transform.localPosition);
    Debug.LogWarning("Cam Pose" + camXRPose.ToString());
    // Instantiate(m_PositionPrefab, camXRPose, Quaternion.identity, m_XRSpace.transform);
    // 6.2 Create Translation Matrix Moving ValidPoseXRSpace to camXRPose
    Debug.LogWarning("XR Pose" + m_XRSpace.transform.position.ToString());
    // Vector3 translation = new Vector3((-camXRPose.x) - validPoseXRSpace.x,
    //                                   (-camXRPose.y) - validPoseXRSpace.y,
    //                                   (-camXRPose.z) - validPoseXRSpace.z);
    Vector3 translation = new Vector3(((-camXRPose.x) - go.transform.localPosition.x),
                                      ((-camXRPose.y) - go.transform.localPosition.y),
                                      ((-camXRPose.z) - go.transform.localPosition.z));
    // Vector3 translation = new Vector3(-(validPoseXRSpace.x - (-camXRPose.x)),
    //                                   -(validPoseXRSpace.y - (-camXRPose.y)),
    //                                   -(validPoseXRSpace.z - (-camXRPose.z)));
    Matrix4x4 transMatrix = Matrix4x4.Translate(translation);
    Debug.LogWarning("Translation" + translation.ToString());
    // 6.3 Calculate the new pose for the xr space and set
    Vector3 newXRSpacePose = transMatrix.MultiplyPoint3x4(m_XRSpace.transform.position);
    Debug.LogWarning("New XR Pose" + newXRSpacePose.ToString());

    // m_XRSpace.transform.SetPositionAndRotation(newXRSpacePose, Quaternion.identity);
    m_XRSpace.transform.position = (newXRSpacePose);
    // m_XRSpace.transform.position = (newXRSpacePose * -1.0f);

    Debug.Log("Distance anchor to cam = " + Vector3.Distance(cam.transform.localPosition, testBeacon.position));
    yield return null;
  }
}
