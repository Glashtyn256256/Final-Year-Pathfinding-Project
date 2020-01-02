using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Container of data with some limited functionality, not in heriting from monobehaviour could make pathfinding more fficient

//A enum that allows us to give our node a type. Blocked is when we have a wall, goal is the goal node and open is the node being walkable.
public enum NodeType
{
    Open = 0,
    Blocked = 1,
    GoalNode = 2
}

public class Node
{
    public NodeType nodeType = NodeType.Open;

    //integers to track position in the 2d array.
    public int xIndexPosition = -1;
    public int yIndexPosition = -1;

    //Keep track of node position
    public Vector3 nodeWorldPosition;

    //neighbour of the node
    public List<Node> neighbours = new List<Node>();

    //This is important for when we get our path.
    public Node nodeParent = null;

    //The gCost is the distance between the current node we're looking at and the start node
    //The hCost is the heurstic, it estimates the distance between the current node we're looking at to the end node.
    //This is the total cost of the node we are in.
    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }
    
    //Constructor
    public Node(int xindexposition, int yindexposition, NodeType nodetype, Vector3 worldposition)
    {
        xIndexPosition = xindexposition;
        yIndexPosition = yindexposition;
        nodeType = nodetype;
        nodeWorldPosition = worldposition;
    }

    //Reset our node parent to null.
    public void Reset()
    {
        nodeParent = null;
    }
}
