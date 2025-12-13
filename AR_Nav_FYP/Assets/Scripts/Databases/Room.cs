using Unity.VisualScripting;
using UnityEngine;
using Immersal.XR;

[RequireComponent(typeof(XRMap))]
public class Room : MonoBehaviour
{
    public string roomId;
    public int floorNumber;

    public string RoomId => roomId;

    // Grab the map id when this script is attached to an XRMap
    void OnValidate()
    {
        if (string.IsNullOrEmpty(roomId))
        {
            roomId = gameObject.GetComponent<XRMap>().mapId.ToString();
            // gameObject.GetComponent<XRMap>().Visualization.Mesh.bounds
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
        }
    }
}
