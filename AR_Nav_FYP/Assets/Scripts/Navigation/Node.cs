using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AR_NAV_FYP.Navigation {
    public class Node : MonoBehaviour {
        public Vector3 position;
        private Vector3 scale;
        public bool isDestinationNode = false;

        [Header("A*")]
        public List<Node> neighbors = new List<Node>();
        public float FCost { get { return GCost + HCost; } } // Combined Hueuristic Cost
        public float HCost { get; set; } // Distance to end node
        public float GCost { get; set; } // Total cost of path so far
        public float Cost { get; set; }

        public Node Parent { get; set; }

        public Node NextInList { get; set; } //next node in navigation list

        public NodeLocation Location;

        private void Awake() {
            //transform.GetChild(0).gameObject.SetActive(false);
            Activate(false);

            position = transform.position;
            scale = transform.localScale;
            //FindNeighbors(1.0f);
        }

        //void Update() {
        //    //make pulsate
        //    if (isDestinationNode)
        //        transform.localScale = scale * (1 + Mathf.Sin(Mathf.PI * Time.time) * .2f);
        //}

        public void Activate(bool active) {
            if (!isDestinationNode) {
                transform.GetChild(0).gameObject.SetActive(active);
                if (NextInList != null) {
                    transform.LookAt(NextInList.transform);
                }
            }
        }

        public void FindNeighbors(float maxDistance) {
            foreach (Node node in FindObjectsOfType<Node>()) {
                if (Vector3.Distance(node.position, this.position) < maxDistance) {
                    neighbors.Add(node);
                }
            }
        }

        public void resetPositionOnLocalization() {
            position = transform.position;
        }

        public bool ApplyWheelChairPolicy() {
            bool stayInGraph;

            switch (Location) {
                case NodeLocation.Stairs:
                    stayInGraph = false;
                    break;
                case NodeLocation.WheelChairInaccessablePath:
                    stayInGraph = false;
                    break;
                default:
                    stayInGraph = true;
                    break;
            }

            //if (profile.wheelchairuser) stayInGraph = false;

            return stayInGraph;
        }
    }

    public enum NodeLocation {
        defualt,
        Stairs,
        Elevator,
        Ramp,
        keyCardDoor,
        WheelChairInaccessablePath
    }

}