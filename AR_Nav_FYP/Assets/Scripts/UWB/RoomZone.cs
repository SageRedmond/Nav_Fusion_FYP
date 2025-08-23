using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomZone : MonoBehaviour
{
  public Vector3[] corners; // Define the polygon vertices
  
  public bool IsObjectInArea(GameObject obj)
  {
    return IsPointInRoom(obj.transform.position, corners);
  }
  
  // Point-in-polygon raycasting algorithm
  private bool IsPointInRoom(Vector3 point, Vector3[] roomCorners)
  {
    bool inside = false;
    int j = roomCorners.Length - 1;

    for (int i = 0; i < roomCorners.Length; i++)
    {
      if ((roomCorners[i].z < point.z && roomCorners[j].z >= point.z) || (roomCorners[j].z < point.z && roomCorners[i].z >= point.z))
      {
        if ((roomCorners[i].x + (point.z - roomCorners[i].z) / (roomCorners[j].z - roomCorners[i].z) * (roomCorners[j].x - roomCorners[i].x)) < point.x)
        {
          inside = true;
        }
      }
      j = i;
    }

    return inside;
  }
}