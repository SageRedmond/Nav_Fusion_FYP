using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class UiPickDestination : MonoBehaviour {
    //public List<Node> DestinationNodes = new List<Node>();
    public List<GameObject> Goals = new List<GameObject>();
    [SerializeField] private RoomNameTracker infoToGive;

    [SerializeField]
    private NavController NavController;

    public GameObject buttonPrefab;
    public Transform buttonParent;
    public ScrollRect scrollRect;
    public GameObject UIGoalPanel;

    //Populate panel with button list of destinations
    private void Awake() {
        PopulatePanelWithButtons();
    }

    void PopulatePanelWithButtons() {
        foreach (GameObject goal in Goals) {
            GameObject button = Instantiate(buttonPrefab, buttonParent);
            button.GetComponentInChildren<TMP_Text>().text = goal.name;
            button.GetComponent<Button>().onClick.AddListener(() => ButtonClicked(goal));
        }

        Canvas.ForceUpdateCanvases();
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, buttonParent.GetComponent<RectTransform>().rect.height);
    }

    void ButtonClicked(GameObject destination) {
        string TargetName = destination.name;
        Debug.Log("Button clicked for: " + TargetName);

        NavController.TargetPosition = destination.transform.position;
        Debug.Log("Position: " +  NavController.TargetPosition);

        string floorLevel = destination.GetComponent<Goal>().Level.ToString();

        infoToGive.DestinationFloorLevelString = string.Concat(floorLevel.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        infoToGive.DestinationFloorLevelEnum = destination.GetComponent<Goal>().Level;
        infoToGive.DestinationName = string.Concat(TargetName.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');

        UIGoalPanel.SetActive(false);
    }
}