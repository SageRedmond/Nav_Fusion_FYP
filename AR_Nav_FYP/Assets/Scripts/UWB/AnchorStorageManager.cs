using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

public class AnchorStorageManager : MonoBehaviour
{
  [SerializeField] private Button AnchorMarkerBtn;
  [SerializeField] private TrackedImageManager imageTracker;
  [HideInInspector] public List<AnchorMarker> sceneAnchorList = new List<AnchorMarker>();
  [SerializeField] private GameObject m_AnchorPrefab = null; // Should have the AnchorMarker script attached

  [FormerlySerializedAs("m_ARSpace")]
  [SerializeField] private Immersal.XR.XRSpace m_XRSpace;

  public Dictionary<string, Vector3> m_Anchors = new Dictionary<string, Vector3>();

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
  }

  [SerializeField] public AnchorSavefile m_AnchorSavefile = new AnchorSavefile();


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

    sceneAnchorList.Clear();

    LoadAnchors();
  }

  public void MarkAnchor()
  {
    // TODO: Ensure Native State has a beaconID
    // TODO: Ensure imageTackerTransform isn't 0,0,0
    Transform imageMarker = imageTracker.imageMarker.transform;
    // By parenting the anchor to the XR space, we automattically convert it's position from the camera space to the XR space
    GameObject go = Instantiate(m_AnchorPrefab, imageMarker.position, Quaternion.identity, m_XRSpace.transform);
    go.GetComponent<AnchorMarker>().StoreContent();
  }

  public void DeleteAllAnchors()
  {
    List<AnchorMarker> copy = new List<AnchorMarker>();

    foreach (AnchorMarker content in sceneAnchorList)
    {
      copy.Add(content);
    }

    foreach (AnchorMarker content in copy)
    {
      content.RemoveContent();
    }
  }

  public void SaveAnchors()
  {
    // m_Positions.Clear();
    m_Anchors.Clear();

    foreach (AnchorMarker anchor in sceneAnchorList)
    {
      m_Anchors.Add(anchor.AnchorID, anchor.transform.localPosition);
      // m_Positions.Add(anchor.transform.localPosition);
    }
    // m_AnchorSavefile.Anchors = m_Anchors;

    // string jsonstring = JsonUtility.ToJson(m_AnchorSavefile);
    // string dataPath = Path.Combine(Application.persistentDataPath, m_Filename);
    // File.WriteAllText(dataPath, jsonstring);

    // TODO: Write out to REST API
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
      ! This allows unsecure HTTP connections.
    */
    string ip = "192.168.0.71:8080";

    UnityWebRequest www = UnityWebRequest.Get($"http://{ip}/anchors");
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
        GameObject go = Instantiate(m_AnchorPrefab, m_XRSpace.transform);
        go.transform.localPosition = anchor.position;
        go.GetComponent<AnchorMarker>().AnchorID = anchor.id;
      }
    }
    coroutine = null;
  }

  public void EraseSave()
  {
    
  }
}