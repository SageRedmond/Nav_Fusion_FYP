using UnityEngine;

public class DatabaseUser : MonoBehaviour
{
    public void floorLevelTest(int level)
    {
        RoomsRegistry.Instance.EnableOnlyFloorNumber(level);
    }
}