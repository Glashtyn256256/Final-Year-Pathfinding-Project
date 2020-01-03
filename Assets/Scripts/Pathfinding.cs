using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;

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

        openList = new List<Node>();
        closedList = new List<Node>();
        pathList = new List<Node>();

        ResetNodePreviousValuetoNull();
    }

    public void ShowColors()
    {
        ShowColors(gridVisualisation);
    }

    void ShowColors(GridVisualisation graphView)
    {
        if (graphView == null)
        {
            return;
        }

        if (openList != null)
        {
            graphView.ColorNodes(openList.ToList(), openListColor);
        }

        if (closedList != null)
        {
            graphView.ColorNodes(closedList, closedListColor);
        }

        if (pathList != null && pathList.Count > 0)
        {
            graphView.ColorNodes(pathList, pathListColor);
        }
        
    }
    public List<Node> FindPath(Vector3 startposition, Vector3 goalposition, int algorithmindex)
    {
        ResetNodePreviousValuetoNull();
        Node startNode = grid.GetNodeFromWorldPoint(startposition);
        Node goalNode = grid.GetNodeFromWorldPoint(goalposition);
       
        openList = new List<Node>();
        closedList = new List<Node>();

        openList.Add(startNode);

        Stopwatch timer = new Stopwatch();
        timer.Start();
        while (openList.Count > 0)
        {
            Node currentNode;
            //Depth takes from the back off the list instead of front.
            if (algorithmindex == 4)
            {
                currentNode = openList[0];
                //Get the lowest fcost in the list, then we check it to see if it has the lowest hcost.
                for (int i = 1; i < openList.Count; i++)
                {
                    if (openList[i].fCost < currentNode.fCost)
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
            }

            switch (algorithmindex)
            {
                case 0:
                    ExpandFrontierBreadth(currentNode);
                    break;
                case 1:
                    DepthExpandFrontier(currentNode);
                    break;
                case 2:
                    ExpandFrontierDijkstra(currentNode);
                    break;
                case 3:
                    break;
                case 4:
                    ExpandFrontierAStar(currentNode, goalNode);
                    break;
                default:
                    break;
            }

            if (openList.Contains(goalNode))
            {
                pathList = GetPathNodes(goalNode);
                timer.Stop();
                UnityEngine.Debug.Log("Pathfinder searchroutine: elapse time = " + (timer.Elapsed.TotalMilliseconds).ToString() + " milliseconds");
                return pathList;
            }
        }
        UnityEngine.Debug.Log("Path is blocked, no path possible to goal");
        return null;
    }

    //These are a waste shorten when you get the chance. A lot of re-used code
   

 
    void DepthExpandFrontier(Node node)
    {
        if (node != null)
        {
            foreach (var neighbour in node.neighbours)
            {
                if (neighbour.nodeType == NodeType.Blocked
                   || closedList.Contains(neighbour)
                   || openList.Contains(neighbour))
                {
                    continue;
                }

                neighbour.nodeParent = node;
                openList.Add(neighbour);
            }
        }
    }

    void ExpandFrontierBreadth(Node node)
    {
        if (node != null)
        {
            foreach (Node neighbour in node.neighbours)
            {
                if (neighbour.nodeType == NodeType.Blocked 
                    || closedList.Contains(neighbour)
                    || openList.Contains(neighbour))
                {
                    continue;
                }

                neighbour.nodeParent = node;
                openList.Add(neighbour);

            }
        }
    }
    void ExpandFrontierDijkstra(Node node)
    {
        if (node != null)
        {
            foreach (Node neighbour in node.neighbours)
            {
                if(neighbour.nodeType == NodeType.Blocked || closedList.Contains(neighbour))
                {
                    continue;
                }

                int distanceToNeighbor = node.gCost + grid.GetNodeDistance(node, neighbour);

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

    void ExpandFrontierAStar(Node node, Node goalnode)
    {
        if (node != null)
        {
            foreach (Node neighbour in node.neighbours)
            {
                if (neighbour.nodeType == NodeType.Blocked || closedList.Contains(neighbour))
                {
                    continue;
                }

                int distanceToNeighbor = node.gCost + grid.GetNodeDistance(node, neighbour);

                if (distanceToNeighbor < neighbour.gCost || !openList.Contains(neighbour))
                {
                    neighbour.gCost = distanceToNeighbor;
                    neighbour.hCost = grid.GetNodeDistance(node, goalnode);
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


    //Without this it crashes due to it adding all the nodes into frontier and bombs out bizzare.
    //One to look at later since I thought they would just be overwritten with the new values but it doesnt seem to be the case.
    void ResetNodePreviousValuetoNull()
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
