using UnityEngine;

// Test class, not part of any user flow
public class DatabaseUser : MonoBehaviour
{
    public void floorLevelTest(int level)
    {
        RoomsRegistry.Instance.EnableOnlyRoomsWithFloorNumber(level);
    }
}