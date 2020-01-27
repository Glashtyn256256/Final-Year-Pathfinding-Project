﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    GridManager grid;
    GridVisualisation gridVisualisation;

    List<Node> openList;
    List<Node> closedList;
    List<Node> pathList;

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
        openList = new List<Node>();
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
        ResetNodeParentgCostAndhCost();
        ClearLists();

        Node startNode = grid.GetNodeFromWorldPoint(startposition);
        Node goalNode = grid.GetNodeFromWorldPoint(goalposition);
        Node currentNode;
        
        Stopwatch timer = new Stopwatch();

        int nodesExploredCount = 0;
        string pathfindingUsed = "";
        startNode.gCost = 0;

        openList.Add(startNode);          
        timer.Start();

        while (openList.Count > 0)
        {
            //Depth takes from the back off the list instead of front.
            if (algorithmindex == 4)
            {
                currentNode = openList[0];
                //Get the lowest fcost in the list.
                for (int i = 1; i < openList.Count; i++)
                {
                    if (openList[i].fCost < currentNode.fCost)
                    {
                            currentNode = openList[i];
                    }
                }
                openList.Remove(currentNode);
            }
            else if (algorithmindex == 2)
            {
                currentNode = openList[0];
                //Get the lowest gcost in the openlist.
                for (int i = 1; i < openList.Count; i++)
                {
                    if (openList[i].gCost < currentNode.gCost)
                    {
                        currentNode = openList[i];
                    }
                }
                openList.Remove(currentNode);
            }
            else if(algorithmindex == 3)
            {
                currentNode = openList[0];
                //Get the lowest hcost in the openlist.
                for (int i = 1; i < openList.Count; i++)
                {
                    if (openList[i].hCost < currentNode.hCost)
                    {
                        currentNode = openList[i];
                    }
                }
                openList.Remove(currentNode);
            }
            else if (!(algorithmindex == 1))
            {
                currentNode = openList[0];
                openList.Remove(currentNode);
            }      
            else
            {
                currentNode = openList.Last();
                openList.Remove(currentNode);
            }
           
            if (!closedList.Contains(currentNode))
            {
                closedList.Add(currentNode);
                nodesExploredCount++;
            }

            switch (algorithmindex)
            {
                case 0:
                    ExpandBreadthFirstSearchOpenList(currentNode);
                    pathfindingUsed = "Breadth First Search with ";
                    break;
                case 1:
                    ExpandDepthFirstSearchOpenList(currentNode);
                    pathfindingUsed = "Depth First Search with ";
                    break;
                case 2:
                    ExpandDijkstraOpenList(currentNode, heuristicindex);
                    pathfindingUsed = "Dijkstra with ";
                    break;
                case 3:
                    ExpandBestFirstSearchOpenList(currentNode, goalNode, heuristicindex);
                    pathfindingUsed = "Best First Search with ";
                    break;
                case 4:
                    ExpandAStarOpenList(currentNode, goalNode, heuristicindex);
                    pathfindingUsed = "A* Pathfinding with ";
                    break;
                default:
                    break;
            }

            if (openList.Contains(goalNode))
            {
                pathList = GetPathNodes(goalNode);
                timer.Stop();
                unitmessage = ((pathfindingUsed) 
                    + (HeuristicUsed(heuristicindex)) 
                    + ("Elapsed time = ")
                    + (timer.Elapsed.TotalMilliseconds).ToString()
                    + " milliseconds , Nodes Explored = " 
                    + nodesExploredCount.ToString()
                    + ", Nodes To Goal = "
                    + pathList.Count.ToString());


                return pathList;
            }
        }
        unitmessage = ("Path is blocked, no path possible to goal");
        return null;
    }

    //These are a waste shorten when you get the chance. A lot of re-used code
   
    void ExpandBreadthFirstSearchOpenList(Node node)
        {
            if (node != null)
            {
                foreach (Node neighbour in node.neighbours)
                {
                    if (neighbour.nodeType != NodeType.Blocked 
                         && !closedList.Contains(neighbour)
                        && !openList.Contains(neighbour))
                    {
                        neighbour.nodeParent = node;
                        openList.Add(neighbour);
                    }
                }
            }
        }
 
    void ExpandDepthFirstSearchOpenList(Node node)
    {
        if (node != null)
        {
            foreach (var neighbour in node.neighbours)
            {
                if (neighbour.nodeType != NodeType.Blocked
                    && !closedList.Contains(neighbour)
                   && !openList.Contains(neighbour))
                {
                    neighbour.nodeParent = node;
                    openList.Add(neighbour);
                }
            }
        }
    }

    
    void ExpandDijkstraOpenList(Node node, int heuristicindex)
    {
        if (node != null)
        {
            foreach (Node neighbour in node.neighbours)
            {
                if(neighbour.nodeType == NodeType.Blocked || closedList.Contains(neighbour))
                {
                    continue;
                }

                float distanceToNeighbor = node.gCost + grid.GetNodeDistance(node, neighbour, heuristicindex) + (int)neighbour.nodeType;

                if (distanceToNeighbor < neighbour.gCost || !openList.Contains(neighbour))
                {
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
                    neighbour.hCost = grid.GetNodeDistance(node, goalnode, heuristicindex)+(int)neighbour.nodeType;
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
                    neighbour.hCost = grid.GetNodeDistance(node, goalnode, heuristicindex) + (int)neighbour.nodeType;
                    neighbour.nodeParent = node;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }

            }
        }
    }
 
    List<Node> GetPathNodes(Node goalnode)
    {
        List<Node> path = new List<Node>();
        if (goalnode == null)
        {
            return path;
        }

        path.Add(goalnode);
        Node currentNode = goalnode.nodeParent;
        while(currentNode != null)
        {
            path.Insert(0, currentNode); //saves us haivng to reverse if we use add
            currentNode = currentNode.nodeParent;
        }

        return path;
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
