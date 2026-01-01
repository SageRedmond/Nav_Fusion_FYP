using Unity.VisualScripting;
using UnityEngine;
using Immersal.XR;

[RequireComponent(typeof(XRMap))]
public class Room : MonoBehaviour
{
    public string roomId;
    public int floorNumber;

    public string RoomId => roomId;

    [SerializeField]
    private XRMap m_mapComponent;

    // Grab the map id when this script is attached to an XRMap
    void OnValidate()
    {
        if (string.IsNullOrEmpty(roomId))
        {
            roomId = gameObject.GetComponent<XRMap>().mapId.ToString();

            m_mapComponent = gameObject.GetComponent<XRMap>();
        }
    }

    void Start()
    {
        // Register this room when scene loads
        RoomsRegistry.Instance.RegisterRoom(this);

        if (m_mapComponent == null)
        {
            m_mapComponent = gameObject.GetComponent<XRMap>();
        }
    }

    public void SetRoomActiveState(bool state)
    {
        if (state != gameObject.activeSelf)
        {
            Debug.Log("Setting Room " + roomId + " to state " + state);
            gameObject.SetActive(state);
            // m_mapComponent.re
            // if (state == false)
            // {
            //     if (m_mapComponent == null)
            //     {
            //         Debug.Log("Map Component Does not Exists");
            //     }

            //     MapManager.RemoveMap(m_mapComponent.mapId, true, false);
            // }
            // else
            // {
            //     // MapManager.LoadMap(m_mapComponent);
            // }
        }
    }
}
