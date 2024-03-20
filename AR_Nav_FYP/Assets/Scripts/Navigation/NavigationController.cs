using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Attached To Player/Indicator

namespace AR_NAV_FYP.Navigation {
    using AR_NAV_FYP.Policy;

    public class NavigationController : MonoBehaviour {
        public AStar AStar;
        public Node target;
        private Transform destination;

        public List<Node> path = new List<Node>();
        public int currNodeIndex = 0;
        private float maxDistance = 1.1f;

        private bool _initialized = false;
        private bool _initializedComplete = false;

        [SerializeField] private UserProfile user;

        
        // Start is called before the first frame update
        void Start() {
#if UNITY_EDITOR
            InitNavigation();
#endif
        }

        public void InitNavigation() {
            if (!_initialized) {
                _initialized = true;
                Node[] allNodes = FindObjectsOfType<Node>();
                Debug.Log("NODES: " + allNodes.Length);

                Node closestNode = ReturnClosestNode(allNodes, transform.position);
                Debug.Log("closest: " + closestNode.gameObject.name);

                //Node target = GameObject.Find("Door").GetComponent<Node>();


                if (user.IsWheelchairUser) {
                    foreach (Node node in allNodes) {
                        if (node.ApplyWheelChairPolicy()) {
                            node.FindNeighbors(maxDistance);
                        }
                    }
                }
                else {
                    foreach (Node node in allNodes) {
                        node.FindNeighbors(maxDistance);
                    }
                }
                


                path = AStar.FindPath(closestNode, target, allNodes);

                if (path == null) {
                    //increase search distance for neighbors
                    maxDistance += .1f;
                    Debug.Log("Increasing search distance: " + maxDistance);
                    _initialized = false;
                    InitNavigation();
                    return;
                }

                //set next nodes 
                for (int i = 0; i < path.Count - 1; i++) {
                    path[i].NextInList = path[i + 1];
                }

                path[0].Activate(true);
                _initializedComplete = true;

            }
        }

        Node ReturnClosestNode(Node[] nodes, Vector3 point) {
            float minDist = Mathf.Infinity;
            Node closestNode = null;
            foreach (Node node in nodes) {
                float dist = Vector3.Distance(node.position, point);
                if (dist < minDist) {
                    closestNode = node;
                    minDist = dist;
                }
            }
            return closestNode;
        }

        private void OnTriggerEnter(Collider other) {
            if (_initializedComplete && other.CompareTag("waypoint")) {
                currNodeIndex = path.IndexOf(other.GetComponent<Node>());
                if (currNodeIndex < path.Count - 1) {
                    path[currNodeIndex + 1].Activate(true);
                }
                path[currNodeIndex].Activate(false);
            }
        }

        public void MakePath() {
            if(path.Count > 0) {
                foreach (Node node in path) {
                    node.Activate(false);
                }
            }
            GameObject[] goals = GameObject.FindGameObjectsWithTag("DestinationNode");
            foreach (GameObject goal in goals) {
                goal.SetActive(false);
            }
            target.gameObject.SetActive(true);

            _initialized = false;
            _initializedComplete = false;
            InitNavigation();
        }
    }
}