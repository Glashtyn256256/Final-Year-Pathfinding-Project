using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class FlowfieldPathfinding : MonoBehaviour
{
    GridManager grid;
    GridVisualisation gridVisualisation;

    List<Node> openList;
    List<Node> closedList;

    public Color startNodeColor = Color.green;
    public Color goalNodeColor = Color.red;


    public bool exitOnGoal = true;
    public bool isComplete = false;

    public void CreatePathfinding(GridManager gridnodes, GridVisualisation gridvisualisation)
    {
        if (gridvisualisation == null || gridnodes == null)
        {
            UnityEngine.Debug.Log("Pathfinder Init error: missing component(s)!");
            return;
        }

        grid = gridnodes;
        gridVisualisation = gridvisualisation;

        ClearLists();
        ResetNodeParentgCostAndhCost();
    }
    public void ClearLists() 
    {
        openList = new List<Node>();
        closedList = new List<Node>();
    }
    public void FlowfieldPath(Vector3 goalposition, bool visualaid)
    {
        ResetNodeParentgCostAndhCost();
        ClearLists();

        Node goalNode = grid.GetNodeFromWorldPoint(goalposition);
        Node currentNode;

        Stopwatch timer = new Stopwatch();

        goalNode.gCost = 0;

        openList.Add(goalNode);
        timer.Start();

        while (openList.Count > 0)
        {

            currentNode = openList[0];
            openList.Remove(currentNode);

            if (!closedList.Contains(currentNode))
            {
                closedList.Add(currentNode);
            }

            CostField(currentNode);
        }
        grid.IntegrationField();
        if (visualaid)
        {
          gridVisualisation.ChangePositionOfArrow();
        }
    }

    void CostField(Node node)
    {
        if (node != null)
        {
            foreach (Node neighbour in node.neighbours)
            {
                if (neighbour.nodeType == NodeType.Blocked || closedList.Contains(neighbour))
                {
                    //if(neighbour.nodeType == NodeType.Blocked)
                    //{
                    //    neighbour.gCost = 60000;
                    //}
                    continue;
                }

                float distanceToNeighbor = node.gCost + grid.GetNodeDistance(node, neighbour) + (int)neighbour.nodeType;

                if (distanceToNeighbor < neighbour.gCost || !openList.Contains(neighbour))
                {
                    neighbour.gCost = distanceToNeighbor;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }
    }

    //Without this it crashes due to it adding all the nodes into frontier and bombs out bizzare.
    //One to look at later since I thought they would just be overwritten with the new values but it doesnt seem to be the case.
    void ResetNodeParentgCostAndhCost()
    {
        for (int x = 0; x < grid.GetGridWidth; x++)
        {
            for (int y = 0; y < grid.GetGridHeight; y++)
            {
                grid.gridNodes[x, y].Reset();
            }
        }
    }
}
