using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class MinHeapPathfinding : MonoBehaviour
{
    GridManager grid;
    GridVisualisation gridVisualisation;

    MinHeap<Node> openList;
    List<Node> closedList;
    List<Node> pathList;
    const int loopAmount = 10100;
    const int averageDivision = 10000;

    public Color startNodeColor = Color.green;
    public Color goalNodeColor = Color.red;
    public Color openListColor = Color.magenta;
    public Color closedListColor = Color.gray;
    public Color pathListColor = Color.cyan;

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
        openList = new MinHeap<Node>(grid.MaxGridSize);
        closedList = new List<Node>();
        pathList = new List<Node>();
    }
    public void ShowPathColors()
    {
        ShowPathColors(gridVisualisation);
    }
    public void ShowSearchColors()
    {
        ShowSearchColors(gridVisualisation);
    }
    void ShowPathColors(GridVisualisation gridvisualisation)
    {
        if (gridvisualisation == null)
        {
            return;
        }

        if (pathList != null && pathList.Count > 0)
        {
            gridvisualisation.ColorNodes(pathList, pathListColor);
        }
    }

    void ShowSearchColors(GridVisualisation gridvisualisation)
    {
        if (gridvisualisation == null)
        {
            return;
        }

        if (openList != null)
        {
            gridvisualisation.ColorNodes(openList.ToList(), openListColor);
        }

        if (closedList != null)
        {
            gridvisualisation.ColorNodes(closedList, closedListColor);
        }
    }
    public List<Node> FindPath(Vector3 startposition, Vector3 goalposition, int algorithmindex, int heuristicindex, out string unitmessage)
    {
        double totalTime = 0;
        int totalGoalNode = 0;
        int totalNodesExplored = 0;
        float nodeTotalGCost = 0;
        // algorithmindex = 4;
        // heuristicindex = 4;
        string pathfindingUsed = "";
        for (int i = 0; i < loopAmount; i++)
        {
            ResetNodeParentgCostAndhCost();
            ClearLists();

            Node startNode = grid.GetNodeFromWorldPoint(startposition);
            Node goalNode = grid.GetNodeFromWorldPoint(goalposition);
            Node currentNode;

            Stopwatch timer = new Stopwatch();

            startNode.gCost = 0;
            startNode.hCost = grid.GetNodeDistance(startNode, goalNode, heuristicindex) + (int)startNode.nodeType;
            startNode.minHeapGTotal = 0;
            openList.Add(startNode);
            timer.Start();

            while (openList.Count() > 0)
            {
                currentNode = openList.RemoveFrontItem();

                if (!closedList.Contains(currentNode))
                {
                    closedList.Add(currentNode);
                }

                switch (algorithmindex)
                {
                    case 0:
                        ExpandDijkstraOpenList(currentNode, heuristicindex);
                        pathfindingUsed = "Dijkstra with ";
                        break;
                    case 1:
                        ExpandBestFirstSearchOpenList(currentNode, goalNode, heuristicindex);
                        pathfindingUsed = "Best First Search with ";
                        break;
                    case 2:
                        ExpandAStarOpenList(currentNode, goalNode, heuristicindex);
                        pathfindingUsed = "A* Pathfinding with ";
                        break;
                    default:
                        break;
                }

                if (openList.Contains(goalNode))
                {
                    timer.Stop();

                    if (i >= 100)
                    {
                        pathList = GetPathNodes(goalNode, out float totalGCost);
                        totalTime = totalTime + timer.Elapsed.TotalMilliseconds;
                        totalGoalNode = totalGoalNode + pathList.Count();
                        totalNodesExplored = totalNodesExplored + closedList.Count();
                        nodeTotalGCost = nodeTotalGCost + totalGCost;

                    }
                    break;
                }
            }
        }

        totalTime /= averageDivision;
        totalGoalNode /= averageDivision;
        totalNodesExplored /= averageDivision;
        nodeTotalGCost /= averageDivision;
        unitmessage = ((pathfindingUsed)
                        + (HeuristicUsed(heuristicindex))
                        + ("Elapsed time = ")
                        + (totalTime.ToString()
                        + " milliseconds , Nodes Explored = "
                        + totalNodesExplored.ToString()
                        + ", Nodes To Goal = "
                        + totalGoalNode.ToString()
                        + " total gcost = "
                        + nodeTotalGCost.ToString()));
        return pathList;
    }

    void ExpandDijkstraOpenList(Node node, int heuristicindex)
    {
        if (node != null)
        {
            foreach (Node neighbour in node.neighbours)
            {
                if (neighbour.nodeType == NodeType.Blocked || closedList.Contains(neighbour))
                {
                    continue;
                }

                float distanceToNeighbor = node.gCost + grid.GetNodeDistance(node, neighbour, heuristicindex) + (int)neighbour.nodeType;

                if (distanceToNeighbor < neighbour.gCost || !openList.Contains(neighbour))
                {
                    neighbour.minHeapGTotal = distanceToNeighbor;
                    neighbour.gCost = distanceToNeighbor;
                    neighbour.nodeParent = node;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }

    }

    void ExpandBestFirstSearchOpenList(Node node, Node goalnode, int heuristicindex)
    {
        if (node != null)
        {
            foreach (Node neighbour in node.neighbours)
            {
                if (neighbour.nodeType != NodeType.Blocked
                          && !closedList.Contains(neighbour)
                         && !openList.Contains(neighbour))
                {
                    float distanceToNeighbor = node.minHeapGTotal + grid.GetNodeDistance(node, neighbour, heuristicindex) + (int)neighbour.nodeType;
                    neighbour.minHeapGTotal = distanceToNeighbor;
                    neighbour.hCost = grid.GetNodeDistance(neighbour, goalnode, heuristicindex) + (int)neighbour.nodeType;
                    neighbour.nodeParent = node;
                    openList.Add(neighbour);
                }
            }
        }
    }

    void ExpandAStarOpenList(Node node, Node goalnode, int heuristicindex)
    {
        if (node != null)
        {
            foreach (Node neighbour in node.neighbours)
            {
                if (neighbour.nodeType == NodeType.Blocked || closedList.Contains(neighbour))
                {
                    continue;
                }

                float distanceToNeighbor = node.gCost + grid.GetNodeDistance(node, neighbour, heuristicindex) + (int)neighbour.nodeType;

                if (distanceToNeighbor < neighbour.gCost || !openList.Contains(neighbour))
                {
                    neighbour.gCost = distanceToNeighbor;
                    neighbour.minHeapGTotal = distanceToNeighbor;
                    neighbour.hCost = grid.GetNodeDistance(neighbour, goalnode, heuristicindex) + (int)neighbour.nodeType;
                    neighbour.nodeParent = node;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }
    }

    List<Node> GetPathNodes(Node goalnode, out float nodegcost)
    {
        nodegcost = 0;
        List<Node> path = new List<Node>();
        if (goalnode == null)
        {
            return path;
        }

        path.Add(goalnode);
        Node currentNode = goalnode.nodeParent;
        while (currentNode != null)
        {
            nodegcost += currentNode.minHeapGTotal;
            path.Insert(0, currentNode); //saves us haivng to reverse if we use add
            currentNode = currentNode.nodeParent;
        }

        return path;
    }

    void AddCurrentNodeToCloseList(Node currentNode)
    {
        if (!closedList.Contains(currentNode))
        {
            closedList.Add(currentNode);
        }
    }
    string HeuristicUsed(int index)
    {
        switch (index)
        {
            case 0:
                return "Diagnol Heuristic: ";
            case 1:
                return "Manhatten Heuristic: ";
            case 2:
                return "Euclidean Heuristic: ";
            case 3:
                return "Chebyshev Heuristic: ";
            case 4:
                return "Octile Heuristic: ";
            case 5:
                return "Custom Distance: ";


        }
        return "error: ";
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
