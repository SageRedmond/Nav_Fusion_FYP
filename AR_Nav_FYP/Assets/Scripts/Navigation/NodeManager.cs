using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public List<Node> nodesList = new List<Node>();

    // Start is called before the first frame update
    void Start() {
        // Find all nodes in the scene and add them to the list
        Node[] nodesInScene = FindObjectsOfType<Node>();
        nodesList.AddRange(nodesInScene);

        //Get Player Room
        //Add too list only nodes in the room
    }

}
