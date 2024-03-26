using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiPickDestination : MonoBehaviour {
    //public List<Node> DestinationNodes = new List<Node>();
    public List<GameObject> Goals = new List<GameObject>();

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
        Debug.Log("Button clicked for: " + destination.name);

        NavController.TargetPosition = destination.transform.position;
        Debug.Log(NavController.TargetPosition);
        //NavController.MakePath();

        UIGoalPanel.SetActive(false);
    }
}