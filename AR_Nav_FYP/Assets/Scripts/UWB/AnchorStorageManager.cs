using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AnchorStorageManager : MonoBehaviour
{
  [SerializeField]
  private Button AnchorMarkerBtn;

  [SerializeField]
  private TrackedImageManager imageTracker;

  [HideInInspector]
  public List<AnchorMarker> contentList = new List<AnchorMarker>();

  [SerializeField]
  private GameObject m_AnchorPrefab = null; // Should have the AnchorMarker script attached

  [FormerlySerializedAs("m_ARSpace")] [SerializeField]
  private Immersal.XR.XRSpace m_XRSpace;

  [SerializeField]
  private string m_Filename = "content.json";
  private Savefile m_Savefile;
  private List<Vector3> m_Positions = new List<Vector3>();

  [System.Serializable]
  public struct Savefile
  {
    //TODO: Assocaite with Beacon ID
      public List<Vector3> positions;
  }

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
    contentList.Clear();
    LoadAnchors();
  }

  // Old Method
  // public void AddContent()
  // {
  //   Transform cameraTransform = Camera.main.transform;
  //   GameObject go = Instantiate(m_AnchorPrefab, cameraTransform.position + cameraTransform.forward, Quaternion.identity, m_XRSpace.transform);
  // }

  /// <summary>
  /// Uses the current pose of the tracked image to position the anchor in XR space
  /// </summary>
  public void MarkAnchor()
  {
    Transform imageMarker = imageTracker.imageMarker.transform;
    // By parenting the anchor to the XR space, we automattically convert it's position from the camera space to the XR space
    GameObject go = Instantiate(m_AnchorPrefab, imageMarker.position, Quaternion.identity, m_XRSpace.transform);
  }

  public void DeleteAllAnchors()
  {
    List<AnchorMarker> copy = new List<AnchorMarker>();

    foreach (AnchorMarker content in contentList)
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
    m_Positions.Clear();
    foreach (AnchorMarker content in contentList)
    {
      m_Positions.Add(content.transform.localPosition);
    }
    m_Savefile.positions = m_Positions;

    string jsonstring = JsonUtility.ToJson(m_Savefile, true);
    string dataPath = Path.Combine(Application.persistentDataPath, m_Filename);
    File.WriteAllText(dataPath, jsonstring);
  }

  public void LoadAnchors()
  {
    string dataPath = Path.Combine(Application.persistentDataPath, m_Filename);
    Debug.LogFormat("Trying to load file: {0}", dataPath);

    try
    {
      Savefile loadFile = JsonUtility.FromJson<Savefile>(File.ReadAllText(dataPath));

      foreach (Vector3 pos in loadFile.positions)
      {
        GameObject go = Instantiate(m_AnchorPrefab, m_XRSpace.transform);
        go.transform.localPosition = pos;
      }

      Debug.Log("Successfully loaded file!");
    }
    catch (FileNotFoundException e)
    {
      Debug.LogWarningFormat("{0}\n.json file for content storage not found. Created a new file!", e.Message);
      File.WriteAllText(dataPath, "");
    }
    catch (NullReferenceException err)
    {
      Debug.LogWarningFormat("{0}\n.json file for content storage not found. Created a new file!", err.Message);
      File.WriteAllText(dataPath, "");
    }
  }
}