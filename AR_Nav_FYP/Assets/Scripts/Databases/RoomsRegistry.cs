using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Immersal.XR;
using Immersal;

public class RoomsRegistry : MonoBehaviour
{
    private static RoomsRegistry instance;
    public static RoomsRegistry Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("RoomsDatabase");
                instance = go.AddComponent<RoomsRegistry>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    void Start()
    {
        StartCoroutine(DisableAllRooms());
    }

    private List<Room> Rooms = new List<Room>();

    public void RegisterRoom(Room room)
    {
        IEnumerable<Room> query = Rooms.Where(r => r.RoomId == room.roomId);

        if (query.Count() < 1)
        {
            Rooms.Add(room);
            Debug.Log($"Registered room: {room.RoomId}");
        }

    }

    public List<Room> GetAll() => new List<Room>(Rooms);

    public Room GetRoomById(string roomId) => Rooms.FirstOrDefault(r => r.RoomId == roomId);
    public int GetFloorNumberByRoomId(string roomId) => Rooms.FirstOrDefault(r => r.RoomId == roomId).floorNumber;

    public List<Room> Query(System.Func<Room, bool> predicate)
    {
        return Rooms.Where(predicate).ToList();
    }

    public void EnableOnlyRoomWithID(string roomId)
    {
        foreach (Room room in Rooms)
        {
            if (room.RoomId == roomId)
            {
                room.SetRoomActiveState(true);
            }
            else
            {
                room.SetRoomActiveState(false);
            }
        }
    }

    public void EnableOnlyRoomsWithFloorNumber(int number)
    {
        foreach (Room room in Rooms)
        {
            if (room.floorNumber == number)
            {
                room.SetRoomActiveState(true);
            }
            else
            {
                room.SetRoomActiveState(false);
            }
        }
    }

    public void EnableAllRooms()
    {
        foreach (Room room in Rooms)
        {
            room.SetRoomActiveState(true);
        }
    }
    System.Collections.IEnumerator DisableAllRooms()
    {
        yield return null; // Wait one frame to allow rooms to register

        foreach (Room room in Rooms)
        {
            room.SetRoomActiveState(false);
        }
        // ImmersalSDK.Instance.RestartSdk();
        // MapManager.RemoveAllMaps(true, false);
    }
}
