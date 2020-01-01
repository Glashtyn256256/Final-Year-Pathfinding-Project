using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;

public class Pathfinder : MonoBehaviour
{
    Grid m_graph;
    GridVisualisation m_graphView;

    List<Node> m_frontierNodes;
    List<Node> m_exploredNodes;
    List<Node> m_pathNodes;
    List<Node> m_frontNodes;

    public Color startColor = Color.green;
    public Color goalColor = Color.red;
    public Color frontierColor = Color.magenta;
    public Color exploredColor = Color.gray;
    public Color pathColor = Color.cyan;
    public Color arrowColor = new Color32(216, 216, 216, 255);
    public Color highLightColor = new Color(1.0f, 1.0f, 0.5f, 1.0f);

    public bool showIterations = true;
    public bool showColors = true;
    public bool showArrows = true;
    public bool exitOnGoal = true;
    public bool isComplete = false;

    public void Init(Grid graph, GridVisualisation graphView)
    {
        if (graphView == null || graph == null)
        {
            UnityEngine.Debug.Log("Pathfinder Init error: missing component(s)!");
            return;
        }

        m_graph = graph;
        m_graphView = graphView;




        m_frontierNodes = new List<Node>();
        //  m_frontierNodes.Enqueue(start);
        m_exploredNodes = new List<Node>();
        m_pathNodes = new List<Node>();
        m_frontNodes = new List<Node>();
        //m_frontNodes.Add(start);


        for (int x = 0; x < m_graph.Width; x++)
        {
            for (int y = 0; y < m_graph.Height; y++)
            {
                m_graph.nodes[x, y].Reset();
            }
        }
    }

    public void ShowColors()
    {
        ShowColors(m_graphView);
    }

    void ShowColors(GridVisualisation graphView)
    {
        if (graphView == null)
        {
            return;
        }

        if (m_frontierNodes != null)
        {
            graphView.ColorNodes(m_frontierNodes.ToList(), frontierColor);
        }

        if (m_exploredNodes != null)
        {
            graphView.ColorNodes(m_exploredNodes, exploredColor);
        }

        if (m_pathNodes != null && m_pathNodes.Count > 0)
        {
            graphView.ColorNodes(m_pathNodes, pathColor);
        }
        
    }
    public List<Node> FindPath(Vector3 startposition, Vector3 goalposition, int algorithmindex)
    {
        ResetNodePreviousValuetoNull();
        Node m_startNode = m_graph.GetNodeFromWorldPoint(startposition);
        Node m_goalNode = m_graph.GetNodeFromWorldPoint(goalposition);
       
        m_frontierNodes = new List<Node>();
        m_exploredNodes = new List<Node>();
        m_pathNodes = new List<Node>();
        m_frontierNodes.Add(m_startNode);

        Stopwatch timer = new Stopwatch();
        timer.Start();
        while (m_frontierNodes.Count > 0)
        {
            Node currentNode;
            //Depth takes from the back off the list instead of front.
            if (!(algorithmindex == 1))
            {
                currentNode = m_frontierNodes[0];
                m_frontierNodes.Remove(currentNode);
            }
            else
            if (algorithmindex == 4)
            {
                currentNode = m_frontierNodes[0];
                //Get the lowest fcost in the list, then we check it to see if it has the lowest hcost.
                for (int i = 1; i < m_frontierNodes.Count; i++)
                {
                    if (m_frontierNodes[i].fCost < currentNode.fCost || m_frontierNodes[i].fCost == currentNode.fCost)
                    {
                        if (m_frontierNodes[i].hCost < currentNode.hCost)
                            currentNode = m_frontierNodes[i];
                    }
                }
            }
            else
            {
                currentNode = m_frontierNodes[m_frontierNodes.Count - 1];
                m_frontierNodes.Remove(currentNode);
            }
           
            if (!m_exploredNodes.Contains(currentNode))
            {
                m_exploredNodes.Add(currentNode);
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

           
                        ExpandFrontierAStar(currentNode, m_goalNode);
                    break;
                default:
                    break;
            }
            ExpandFrontierBreadth(currentNode);

            if (m_frontierNodes.Contains(m_goalNode))
            {
                m_pathNodes = GetPathNodes(m_goalNode);
                timer.Stop();
                UnityEngine.Debug.Log("Pathfinder searchroutine: elapse time = " + (timer.ElapsedMilliseconds).ToString() + " milliseconds");
                return m_pathNodes;
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
            for (int i = 0; i < node.neighbours.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbours[i]) &&
                    (!m_frontNodes.Contains(node.neighbours[i])
                    && node.neighbours[i].nodeType != NodeType.Blocked))
                {
                    node.neighbours[i].nodeParent = node;
                    m_frontNodes.Add(node.neighbours[i]);
                }
            }
        }
    }

    void ShowDiagnostics(Node m_goalnode)
    {
        if (showColors)
        {
            ShowColors();
        }
    }

    void ExpandFrontierBreadth(Node node)
    {
        if (node != null)
        {
            foreach (Node neighbour in node.neighbours)
            {
                if (neighbour.nodeType == NodeType.Blocked 
                    || m_exploredNodes.Contains(neighbour)
                    || m_frontierNodes.Contains(neighbour))
                {
                    continue;
                }

                neighbour.nodeParent = node;
                m_frontierNodes.Add(neighbour);

            }

        
    
            //for (int i = 0; i < node.neighbours.Count; i++)
            //{
            //    if (!m_exploredNodes.Contains(node.neighbours[i]) && 
            //        (!m_frontierNodes.Contains(node.neighbours[i])
            //        && node.neighbours[i].nodeType != NodeType.Blocked)) 
            //    {
            //        node.neighbours[i].nodeParent = node;
            //        m_frontierNodes.Add(node.neighbours[i]);
            //    }
            //}
        }
    }
    void ExpandFrontierDijkstra(Node node)
    {
        if (node != null)
        {
            foreach (Node neighbour in node.neighbours)
            {
                if(neighbour.nodeType == NodeType.Blocked || m_exploredNodes.Contains(neighbour))
                {
                    continue;
                }

                int distanceToNeighbor = node.gCost + m_graph.GetNodeDistance(node, neighbour);

                if (distanceToNeighbor < neighbour.gCost || !m_frontierNodes.Contains(neighbour))
                {
                    neighbour.gCost = distanceToNeighbor;
                    neighbour.nodeParent = node;

                    if (!m_frontierNodes.Contains(neighbour))
                    {
                        m_frontierNodes.Add(neighbour);
                    }
                }
                
            }
        }
    }

    void ExpandFrontierAStar(Node node, Node goalNode)
    {
        if (node != null)
        {
            foreach (Node neighbour in node.neighbours)
            {
                if (neighbour.nodeType == NodeType.Blocked || m_exploredNodes.Contains(neighbour))
                {
                    continue;
                }

                int distanceToNeighbor = node.gCost + m_graph.GetNodeDistance(node, neighbour);

                if (distanceToNeighbor < neighbour.gCost || !m_frontierNodes.Contains(neighbour))
                {
                    neighbour.gCost = distanceToNeighbor;
                    neighbour.hCost = m_graph.GetNodeDistance(node, goalNode);
                    neighbour.nodeParent = node;

                    if (!m_frontierNodes.Contains(neighbour))
                    {
                        m_frontierNodes.Add(neighbour);
                    }
                }

            }
        }
    }
 
    List<Node> GetPathNodes(Node endNode)
    {
        List<Node> path = new List<Node>();
        if (endNode == null)
        {
            return path;
        }

        path.Add(endNode);
        Node currentNode = endNode.nodeParent;
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
        for (int x = 0; x < m_graph.Width; x++)
        {
            for (int y = 0; y < m_graph.Height; y++)
            {
                m_graph.nodes[x, y].Reset();
            }
        }
    }
}
