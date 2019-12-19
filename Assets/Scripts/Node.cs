using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Container of data with some limited functionality, not in heriting from monobehaviour could make pathfinding more fficient

public enum NodeType
{
    Open = 0,
    Blocked = 1
}

public class Node
{
    public NodeType nodeType = NodeType.Open;

    //integers to track position in the 2d array.
    public int xIndex = -1;
    public int yIndex = -1;

    //Keep track of node position
    public Vector3 position;

    //neighbour of the node
    public List<Node> neighbours = new List<Node>(); 

    public Node previous = null;

    //Constructor
    public Node(int xIndex, int yIndex, NodeType nodeType)
    {
        this.xIndex = xIndex;
        this.yIndex = yIndex;
        this.nodeType = nodeType;
    }

    public void Reset()
    {
        previous = null;
    }
}
