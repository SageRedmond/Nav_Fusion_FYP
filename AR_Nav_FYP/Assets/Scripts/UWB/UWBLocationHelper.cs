using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Immersal.XR;

public class UWBLocationHelper : MonoBehaviour
{
  [SerializeField]
  private AnchorStorageManager anchorManager;

  private static float FLOOR_HEIGHT_CONSTRAINT = 0.0f;
  private static float CEILING_HEIGHT_CONSTRAINT = 0.0f;

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

  public static SphereLocation CartesianToSpherical(Vector3 v, Sphere sphere) {

      SphereLocation result = new SphereLocation();

      if (v.x == 0) {
        v.x = Mathf.Epsilon;
      }
      result.latitude = Mathf.Atan(v.z / v.x);

      if (v.x < 0) {
        result.latitude += Mathf.PI;
      }        

      result.longitude = Mathf.Asin(v.y / sphere.raduis);

      return result;
    }


    public static Vector3 SphericalToCartesian(float latitude, float longitude, Sphere sphere) {   
        float a = sphere.raduis * Mathf.Cos(longitude);
        float x = a * Mathf.Cos(latitude);
        float y = sphere.raduis * Mathf.Sin(longitude);
        float z = a * Mathf.Sin(latitude);

        return new Vector3(x, y, z);     
    }
}
