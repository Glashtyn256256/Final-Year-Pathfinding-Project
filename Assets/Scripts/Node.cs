using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Container of data with some limited functionality, not in heriting from monobehaviour could make pathfinding more fficient

//A enum that allows us to give our node a type. Blocked is when we have a wall, goal is the goal node and open is the node being walkable.
public enum NodeType
{
    Open = 0,
    Blocked = 1,  
    Grass = 3,
    Water = 15,
    GoalNode = 10
}

public class Node : IHeapItem<Node>
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
    public float gCost;
    public float hCost;
    public float fCost { get { return gCost + hCost; } }

    public bool UnitAbove;

    public int heapIndex;

    //Constructor
    public Node(int xindexposition, int yindexposition, NodeType nodetype, Vector3 worldposition)
    {
        xIndexPosition = xindexposition;
        yIndexPosition = yindexposition;
        nodeType = nodetype;
        nodeWorldPosition = worldposition;
    }
    public int HeapIndex
    {
        get { return heapIndex;  }
        set { heapIndex = value; }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = 0;
        if (!float.IsInfinity(fCost))
        {
            compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0)
            {
                compare = hCost.CompareTo(nodeToCompare.hCost);
            }
        }
        else if (!float.IsInfinity(hCost))
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        else if (!float.IsInfinity(gCost))
        {
            compare = gCost.CompareTo(nodeToCompare.gCost);
        }
        return -compare;
    }

    //Reset our node parent to null.
    public void Reset()
    {
        nodeParent = null;
        //compareValue = Mathf.Infinity;
        gCost = Mathf.Infinity;
        hCost = Mathf.Infinity;
        UnitAbove = false;
    }
}
