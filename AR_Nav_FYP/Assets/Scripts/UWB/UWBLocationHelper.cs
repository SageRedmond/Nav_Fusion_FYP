using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Immersal.XR;

public class UWBLocationHelper : MonoBehaviour
{
  [SerializeField]
  private AnchorStorageManager anchorManager;

  [SerializeField]
  private XRSpace m_XRSpace;

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

  private IEnumerator ComputeLocalisationHint()
  {
    NativeState state = NativeStateManager.State;
    string id = state.beaconId;
    if (id == "")
    {
      yield break;
    }

    float sphereRadius = state.distance;


  }

  private Vector3 XRSpaceToUnity(Transform XRSpace, Matrix4x4 XRSpaceOffset, Vector3 pos) {
    Matrix4x4 m = XRSpace.worldToLocalMatrix;
    pos = m.MultiplyPoint(pos);
    pos = XRSpaceOffset.MultiplyPoint(pos);
    return pos;
  }

  private Vector3 XRSpaceToUnity(Transform XRSpace, Vector3 pos) {
    pos = XRSpaceToUnity(XRSpace, Matrix4x4.identity, pos);
    return pos;
  }

  public static SphereLocation CartesianToSpherical(Vector3 v, Sphere sphere)
  {

    SphereLocation result = new SphereLocation();

    if (v.x == 0)
    {
      v.x = Mathf.Epsilon;
    }
    result.latitude = Mathf.Atan(v.z / v.x);

    if (v.x < 0)
    {
      result.latitude += Mathf.PI;
    }

    result.longitude = Mathf.Asin(v.y / sphere.raduis);

    return result;
  }

  public static Vector3 SphericalToCartesian(float latitude, float longitude, Sphere sphere)
  {
    float a = sphere.raduis * Mathf.Cos(longitude);
    float x = a * Mathf.Cos(latitude);
    float y = sphere.raduis * Mathf.Sin(longitude);
    float z = a * Mathf.Sin(latitude);

    return new Vector3(x, y, z);
  }
}
