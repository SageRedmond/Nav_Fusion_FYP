using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AR_NAV_FYP.Navigation {
    public class DestinationPicker : MonoBehaviour
    {
        public List<Node> DestinationNodes = new List<Node>();

        [SerializeField]
        private NavigationController NavController;

        public GameObject buttonPrefab;
        public Transform buttonParent;
        public ScrollRect scrollRect;
        public GameObject UIGoalPanel;

        //Populate panel with button list of destinations
        private void Awake() {
            //GameObject[] destinations = GameObject.FindGameObjectsWithTag("DestinationNode");
            //DestinationNodes.AddRange(destinations);

            PopulatePanelWithButtons();
        }

        void PopulatePanelWithButtons() {
            foreach(Node destination in DestinationNodes) {
                GameObject button = Instantiate(buttonPrefab, buttonParent);
                button.GetComponentInChildren<TMP_Text>().text = destination.name;
                button.GetComponent<Button>().onClick.AddListener(() => ButtonClicked(destination));
            }

            Canvas.ForceUpdateCanvases();
            scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, buttonParent.GetComponent<RectTransform>().rect.height);
        }

        void ButtonClicked(Node destination) {
            Debug.Log("Button clicked for: " + destination.name);

            NavController.target = destination;
            NavController.MakePath();

            UIGoalPanel.SetActive(false);
        }
    }
}