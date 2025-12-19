using Unity.VisualScripting;
using UnityEngine;
using Immersal.XR;

[RequireComponent(typeof(XRMap))]
public class Room : MonoBehaviour
{
    public string roomId;
    public int floorNumber;

    public string RoomId => roomId;

    private XRMap m_mapComponent;
    // Grab the map id when this script is attached to an XRMap
    void OnValidate()
    {
        if (string.IsNullOrEmpty(roomId))
        {
            roomId = gameObject.GetComponent<XRMap>().mapId.ToString();
            // gameObject.GetComponent<XRMap>().Visualization.Mesh.bounds
            m_mapComponent = gameObject.GetComponent<XRMap>();
        }
    }

    void Start()
    {
        // Register this room when scene loads
        RoomsRegistry.Instance.RegisterRoom(this);
    }

    public void SetRoomActiveState(bool state)
    {
        if (state != gameObject.activeSelf)
        {
            Debug.Log("Setting Room " + roomId + " to state " + state);
            gameObject.SetActive(state);
            // gameObject.GetComponent<XRMap>().

            if (state == false)
            {
                if (MapManager.TryGetMapEntry(m_mapComponent.mapId, out MapEntry entry))
                {
                    MapManager.RemoveMap(m_mapComponent.mapId);
                }
            }
            else
            {
                MapManager.LoadMap(m_mapComponent);
            }
        }
    }
}
