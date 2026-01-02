using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Immersal.XR;
using Immersal;

public class RoomsRegistry : MonoBehaviour
{
    [SerializeField] private Localizer immersalLocalizer;
    [SerializeField] private ILocalizationMethod deviceLocaliser;

    private static RoomsRegistry instance;
    public static RoomsRegistry Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("RoomsRegistry");
                instance = go.AddComponent<RoomsRegistry>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    void Start()
    {
        if (immersalLocalizer == null)
        {
            immersalLocalizer = FindFirstObjectByType<Immersal.XR.Localizer>();
        }
        if (deviceLocaliser == null)
        {
            deviceLocaliser = GameObject.Find("DeviceLocalization").GetComponent<ILocalizationMethod>();
        }
        // StartCoroutine(DisableAllRoomsOnStartup());
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

        // XRMap[] mapsEnabled = { GetRoomById(roomId).m_mapComponent };
        // XRMap[] mapsDisabled = Query(r => r.RoomId != roomId).Select(s => s.m_mapComponent).ToArray();

        // ConfigureLocaliserWithNewMaps(mapsEnabled, mapsDisabled);
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

        // XRMap[] mapsEnabled = Query(r => r.floorNumber == number).Select(s => s.m_mapComponent).ToArray();
        // XRMap[] mapsDisabled = Query(r => r.floorNumber != number).Select(s => s.m_mapComponent).ToArray();

        // ConfigureLocaliserWithNewMaps(mapsEnabled, mapsDisabled);
    }

    public void EnableAllRooms()
    {
        foreach (Room room in Rooms)
        {
            room.SetRoomActiveState(true);
        }

        // XRMap[] mapsEnabled = GetAll().Select(s => s.m_mapComponent).ToArray();
        // ConfigureLocaliserWithNewMaps(mapsEnabled, null);
    }

    public void DisableAllRooms()
    {
        foreach (Room room in Rooms)
        {
            room.SetRoomActiveState(false);
        }
        // XRMap[] mapsDisabled = GetAll().Select(s => s.m_mapComponent).ToArray();
        // ConfigureLocaliserWithNewMaps(null, mapsDisabled);
    }

    System.Collections.IEnumerator DisableAllRoomsOnStartup()
    {
        yield return null; // Wait one frame to allow rooms to register

        foreach (Room room in Rooms)
        {
            room.SetRoomActiveState(false);
        }

        // XRMap[] mapsDisabled = GetAll().Select(s => s.m_mapComponent).ToArray();
        // ConfigureLocaliserWithNewMaps(null, mapsDisabled);
    }

    private void ConfigureLocaliserWithNewMaps(XRMap[] mapsEnabled, XRMap[] mapsDisabled)
    {
        Dictionary<ILocalizationMethod, XRMap[]> enabledMaps = new Dictionary<ILocalizationMethod, XRMap[]> { { deviceLocaliser, mapsEnabled } };
        Dictionary<ILocalizationMethod, XRMap[]> disabledMaps = new Dictionary<ILocalizationMethod, XRMap[]> { { deviceLocaliser, mapsDisabled } };

        DefaultLocalizerConfiguration config = new DefaultLocalizerConfiguration
        {
            ConfigurationsToAdd = enabledMaps,
            ConfigurationsToRemove = disabledMaps,
            StopRunningTasks = true
        };

        try
        {
            immersalLocalizer.ConfigureLocalizer(config);
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }
}
