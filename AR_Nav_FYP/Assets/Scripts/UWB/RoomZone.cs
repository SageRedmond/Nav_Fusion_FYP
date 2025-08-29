using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class RoomZone : MonoBehaviour
{
  public string RoomName;
  [SerializeField] private GameObject[] cornerObjects; // Define the polygon vertices

  public bool CheckPoseInRoom(Vector3 pose)
  {
    Vector3[] roomCorners = cornerObjects.Select(corner => corner.transform.position).ToArray();
    if (roomCorners.Length <= 2)
    {
      Debug.LogError("Not enough vertices for point-in-polygon");
      return false;
    }
    return PointInPolygonAlgorithm(pose, roomCorners);
  }
  public void CheckObjectInRoom(GameObject obj)
  {
    Vector3[] roomCorners = cornerObjects.Select(corner => corner.transform.position).ToArray();
    Debug.Log(PointInPolygonAlgorithm(obj.transform.position, roomCorners));
  }

  public bool IsObjectInArea(GameObject obj)
  {
    Vector3[] roomCorners = cornerObjects.Select(corner => corner.transform.position).ToArray();
    if (roomCorners.Length <= 2)
    {
      Debug.LogError("Not enough vertices for point-in-polygon");
      return false;
    }
    return PointInPolygonAlgorithm(obj.transform.position, roomCorners);
  }

  // www.geeksforgeeks.org/cpp/point-in-polygon-in-cpp/#method-2-using-winding-number-algorithm
  // z is being used for vertical axis instead of y
  private bool PointInPolygonAlgorithm(Vector3 point, Vector3[] roomCorners)
  {
    // Vector3[] roomCorners = corners.Select(corner => corner.transform.position).ToArray();

    int n = roomCorners.Length;
    // Count of intersections
    int count = 0;
    // Iterate through each edge of the polygon
    for (int i = 0; i < n; i++)
    {
      Vector3 p1 = roomCorners[i];
      // Ensure the last point connects to the first point
      Vector3 p2 = roomCorners[(i + 1) % n];

      // Check if the point's y-coordinate is within the
      // edge's y-range and if the point is to the left of
      // the edge
      if ((point.z > Math.Min(p1.z, p2.z))
        && (point.z <= Math.Max(p1.z, p2.z))
        && (point.x <= Math.Max(p1.x, p2.x)))
      {
        // Calculate the x-coordinate of the
        // intersection of the edge with a horizontal
        // line through the point
        double xIntersect = (point.z - p1.z)
                            * (p2.x - p1.x)
                            / (p2.z - p1.z)
                            + p1.x;
        // If the edge is vertical or the point's
        // x-coordinate is less than or equal to the
        // intersection x-coordinate, increment count
        if (p1.x == p2.x || point.x <= xIntersect)
        {
          count++;
        }
      }
    }

    // If the number of intersections is odd, the point is
    // inside the polygon
    return count % 2 == 1;
  }

  public void TestPolygon()
  {
    Vector3[] corners = new[] { new Vector3(1.0f, 0, 1.0f), new Vector3(1.0f, 0, 5.0f), new Vector3(5.0f, 0, 5.0f), new Vector3(5.0f, 0, 1.0f) };

    Vector3 point = new Vector3(6.0f, 0.0f, 6.0f); // Should return false

    bool result = PointInPolygonAlgorithm(point, corners);
    Debug.Log("PnP Test: " + result);
  }
}