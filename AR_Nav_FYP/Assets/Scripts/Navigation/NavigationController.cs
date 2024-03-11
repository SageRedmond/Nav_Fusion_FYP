using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Attached To Player/Indicator

public class NavigationController : MonoBehaviour
{
    public AStar AStar;
    public Node target;
    private Transform destination;

    private List<Node> path = new List<Node>();
    private int currNodeIndex = 0;
    private float maxDistance = 1.1f;

    private bool _initialized = false;
    private bool _initializedComplete = false;

    // Start is called before the first frame update
    void Start() {
        InitNavigation();
    }

    void InitNavigation() {
        if (!_initialized) {
            _initialized = true;
            Node[] allNodes = FindObjectsOfType<Node>();
            Debug.Log("NODES: " + allNodes.Length);

            Node closestNode = ReturnClosestNode(allNodes, transform.position);
            Debug.Log("closest: " + closestNode.gameObject.name);

            //Node target = GameObject.Find("Door").GetComponent<Node>();

            foreach (Node node in allNodes) {
                node.FindNeighbors(maxDistance);
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

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (currNodeIndex < path.Count - 1) {
                currNodeIndex++;
                path[currNodeIndex].Activate(true);
            }
        }
        
    }
}
