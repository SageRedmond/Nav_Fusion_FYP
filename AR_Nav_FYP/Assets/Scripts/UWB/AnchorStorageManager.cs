using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Firebase.Database;
using System.Threading.Tasks;

public class AnchorStorageManager : MonoBehaviour
{
  [SerializeField] private Button AnchorMarkerBtn;
  [SerializeField] private TrackedImageManager imageTracker;
  [HideInInspector] public List<AnchorMarker> sceneAnchorList = new List<AnchorMarker>();
  [SerializeField] private GameObject m_AnchorPrefab = null; // Should have the AnchorMarker script attached

  [FormerlySerializedAs("m_ARSpace")]
  [SerializeField] private Immersal.XR.XRSpace m_XRSpace;

  [SerializeField]
  private AnchorSavefile m_AnchorSavefile;

  public Dictionary<string, Vector3> m_Anchors = new Dictionary<string, Vector3>();

  [System.Serializable]
  public struct AnchorSavefile
  {
    // public List<Vector3> positions;
    public Dictionary<string, Vector3> Anchors;
  }

  private FirebaseDatabase m_Database;
  private const string ANCHOR_KEY = "UWB_ANCHORS";
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

  private void Start()
  {
    m_Database = FirebaseDatabase.DefaultInstance;

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

    m_AnchorSavefile.Anchors = m_Anchors;

    string jsonstring = JsonUtility.ToJson(m_AnchorSavefile, true);
    // string dataPath = Path.Combine(Application.persistentDataPath, m_Filename);
    // File.WriteAllText(dataPath, jsonstring);

    m_Database.GetReference(ANCHOR_KEY).SetRawJsonValueAsync(jsonstring);
  }

  public void LoadAnchors()
  {
    if (coroutine == null)
    {
      coroutine = StartCoroutine(LoadAnchorsCoroutine());
    }
  }

  private IEnumerator LoadAnchorsCoroutine()
  {
    var loadAnchorsDatabaseTask = LoadAnchorsDatabaseAsync();
    yield return new WaitUntil(() => loadAnchorsDatabaseTask.IsCompleted);
    var anhcorData = loadAnchorsDatabaseTask.Result;
    if (anhcorData.HasValue)
    {
      foreach (var (anchorId, pose) in anhcorData.Value.Anchors)
      {
        GameObject go = Instantiate(m_AnchorPrefab, m_XRSpace.transform);
        go.transform.localPosition = pose;
        go.GetComponent<AnchorMarker>().AnchorID = anchorId;
      }
    }
    coroutine = null;
  }

  public async Task<AnchorSavefile?> LoadAnchorsDatabaseAsync()
  {
    var databaseSnapshot = await m_Database.GetReference(ANCHOR_KEY).GetValueAsync();

    if (!databaseSnapshot.Exists)
    {
      return null;
    }

    return JsonUtility.FromJson<AnchorSavefile>(databaseSnapshot.GetRawJsonValue());
  }

  public async Task<bool> CheckSaveExists()
  {
    var databaseSnapshot = await m_Database.GetReference(ANCHOR_KEY).GetValueAsync();
    return databaseSnapshot.Exists;
  }

  public void EraseSave()
  {
    
  }
}