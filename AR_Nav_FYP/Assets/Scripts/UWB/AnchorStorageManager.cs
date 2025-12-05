using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Linq;

public class AnchorStorageManager : MonoBehaviour
{
  [SerializeField] private Button AnchorMarkerBtn;
  [SerializeField] private TrackedImageManager imageTracker;
  // [HideInInspector] public List<AnchorMarker> sceneAnchorList = new List<AnchorMarker>();
  [SerializeField] private GameObject m_AnchorPrefab = null; // Should have the AnchorMarker script attached

  [FormerlySerializedAs("m_ARSpace")]
  [SerializeField] private Immersal.XR.XRSpace m_XRSpace;

  // JSON Class
  [System.Serializable]
  public class AnchorSavefile
  {
    public Anchor[] anchors;
  }
  [System.Serializable]
  public class Anchor
  {
    public string id;
    public Vector3 position;
    public Vector3 rotation;
    public string roomName;
  }

  [SerializeField] public AnchorSavefile m_AnchorSavefile = new AnchorSavefile();
  
  private string MAC_IP = "149.157.140.171:8080";

  private Coroutine coroutine;

  public static AnchorStorageManager Instance
  {
    get
    {
#if UNITY_EDITOR
      if (instance == null && !Application.isPlaying)
      {
        instance = UnityEngine.Object.FindObjectOfType<AnchorStorageManager>();
      }
#endif
      if (instance == null)
      {
        Debug.LogError("No ContentStorageManager instance found. Ensure one exists in the scene.");
      }
      return instance;
    }
  }

  private static AnchorStorageManager instance = null;

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
    if (instance != this)
    {
      Debug.LogError("There must be only one ContentStorageManager object in a scene.");
      UnityEngine.Object.DestroyImmediate(this);
      return;
    }

    if (m_XRSpace == null)
    {
      m_XRSpace = GameObject.FindObjectOfType<Immersal.XR.XRSpace>();
    }

    Button btn = AnchorMarkerBtn.GetComponent<Button>();
    btn.onClick.AddListener(MarkAnchor);
  }

  public void Start()
  {
    Debug.Log("Starting Database");

    // sceneAnchorList.Clear();

    LoadAnchors();
  }

  public void MarkAnchor()
  {
    Debug.Log("Marking Anchor");
    // TODO: Ensure Native State has a beaconID
    // TODO: Ensure imageTackerTransform isn't 0,0,0
    Transform imageMarker = imageTracker.imageMarker.transform;
    // By parenting the anchor to the XR space, we automattically convert it's position from the camera space to the XR space
    GameObject go = Instantiate(m_AnchorPrefab, imageMarker.position, Quaternion.identity, m_XRSpace.transform);
    NativeState state = NativeStateManager.State;
    string AnchorID = state.beaconId;
    go.GetComponent<AnchorMarker>().AnchorID = AnchorID;

    Anchor newAnchor = new Anchor();
    newAnchor.id = AnchorID;
    newAnchor.position = go.transform.position;
    newAnchor.rotation = go.transform.rotation.eulerAngles;
    newAnchor.roomName = "";

    SaveAnchor(newAnchor);
  }

  // public void DeleteAllAnchors()
  // {
  //   List<AnchorMarker> copy = new List<AnchorMarker>();

  //   foreach (AnchorMarker content in sceneAnchorList)
  //   {
  //     copy.Add(content);
  //   }

  //   foreach (AnchorMarker content in copy)
  //   {
  //     content.RemoveContent();
  //   }
  // }

  public void SaveAnchor(Anchor anchorObject)
  {
    Debug.Log("Saving Anchor");
    if (coroutine == null)
    {
      coroutine = StartCoroutine(SaveAnchorCoroutine(anchorObject));
    }
  }

  private IEnumerator SaveAnchorCoroutine(Anchor anchorObject)
  {
    // string ip = "192.168.0.71:8080";
    // string url = "http://192.168.0.25:8080/anchors";
    string url = $"http://{MAC_IP}/anchors";
    string anchorJsonString = JsonUtility.ToJson(anchorObject);
    Debug.Log(anchorJsonString);
    // UnityWebRequest www = UnityWebRequest.Put($"http://{ip}/anchors", anchorJsonString);
    var request = new UnityWebRequest(url, "PUT");
    byte[] bodyRaw = Encoding.UTF8.GetBytes(anchorJsonString);
    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");

    yield return request.SendWebRequest();
    if (request.result != UnityWebRequest.Result.Success)
    {
      Debug.Log(request.error);
    }
    else
    {
      Debug.Log("Anchor upload complete!");
    }

    coroutine = null;
  }

  public void LoadAnchors()
  {
    Debug.Log("Loading Database");
    if (coroutine == null)
    {
      coroutine = StartCoroutine(LoadAnchorsCoroutine());
    }
  }

  private IEnumerator LoadAnchorsCoroutine()
  {
    /* 
      ! NOTE: I've enbled "Allows downloads over HTTP" in player settings for dev builds. 
      ! This allows unsecure HTTP connections!
    */

    UnityWebRequest www = UnityWebRequest.Get($"http://{MAC_IP}/anchors");
    yield return www.SendWebRequest();
    if (www.result != UnityWebRequest.Result.Success)
    {
      Debug.Log("REST error");
      Debug.Log(www.error);
    }
    else
    {
      // Show results as text
      Debug.Log("REST Success");
      Debug.Log(www.downloadHandler.text);
      string requestResult = www.downloadHandler.text;
      m_AnchorSavefile = JsonUtility.FromJson<AnchorSavefile>(requestResult);
      // Or retrieve results as binary data
      // byte[] results = www.downloadHandler.data;

      foreach (Anchor anchor in m_AnchorSavefile.anchors)
      {
        // GameObject go = Instantiate(m_AnchorPrefab, m_XRSpace.transform);
        // go.transform.localPosition = anchor.position;
        // AnchorMarker marker = go.GetComponent<AnchorMarker>();
        // marker.AnchorID = anchor.id;
        SpawnLoadedAnchor(anchor);
        // TODO: Add to scenen list

      }
    }
    coroutine = null;
  }

  public void SpawnLoadedAnchor(Anchor anchorObject)
  {
    GameObject go = Instantiate(m_AnchorPrefab, m_XRSpace.transform);
    // go.transform.localPosition = anchorObject.position;
    Quaternion rotation = Quaternion.Euler(anchorObject.rotation);

    go.transform.SetLocalPositionAndRotation(anchorObject.position, rotation);

    AnchorMarker marker = go.GetComponent<AnchorMarker>();
    marker.AnchorID = anchorObject.id;
  }

  public void EditorLoadAnchors(string requestResultJson)
  {
    AnchorSavefile savefile = JsonUtility.FromJson<AnchorSavefile>(requestResultJson);

    foreach (Anchor anchor in savefile.anchors)
    {
      SpawnLoadedAnchor(anchor);
    }
  }

  public Vector3? GetAnchorPoseByID(string id)
  {
    IEnumerable<Anchor> query = m_AnchorSavefile.anchors.Where(a => a.id == id);

    if (query.Count() > 0)
    {
      return query.FirstOrDefault().position;
    }
    else
    {
      return null;
    }
  }
}