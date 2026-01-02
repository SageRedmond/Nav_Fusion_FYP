using UnityEngine;

// Test class, not part of any user flow
public class DatabaseUser : MonoBehaviour
{
    public void floorLevelTest(int level)
    {
        RoomsRegistry.Instance.EnableOnlyRoomsWithFloorNumber(level);
    }

    public void roomIDTest()
    {
        RoomsRegistry.Instance.EnableOnlyRoomWithID("96011");
    }

    public void AllRoomDisabledTest()
    {
        RoomsRegistry.Instance.DisableAllRooms();
    }
    public void AllRoomEnabledTest()
    {
        RoomsRegistry.Instance.EnableAllRooms();
    }
}