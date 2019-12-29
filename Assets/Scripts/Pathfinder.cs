using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder : MonoBehaviour
{
    Node m_startNode;
    Node m_goalNode;
    Grid m_graph;
    GridVisualisation m_graphView;

    Queue<Node> m_frontierNodes;
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
    int m_iterations = 0;

    public void Init(Grid graph, GridVisualisation graphView, Node start, Node goal)
    {
        if (start == null || goal == null || graphView == null || graph == null)
        {
            Debug.Log("Pathfinder Init error: missing component(s)!");
            return;
        }

        if (start.nodeType == NodeType.Blocked || goal.nodeType == NodeType.Blocked)
        {
            Debug.LogWarning("Start and goal must be unblocked");
            return;
        }

        m_graph = graph;
        m_graphView = graphView;
        m_startNode = start;
        m_goalNode = goal;

        

        m_frontierNodes = new Queue<Node>();
        m_frontierNodes.Enqueue(start);
        m_exploredNodes = new List<Node>();
        m_pathNodes = new List<Node>();
        m_frontNodes = new List<Node>();
        m_frontNodes.Add(start);
        ShowColors(graphView, start, goal);

        for (int x = 0; x < m_graph.Width; x++)
        {
            for (int y = 0; y < m_graph.Height; y++)
            {
                m_graph.nodes[x, y].Reset();
            }
        }

        isComplete = false;
        m_iterations = 0;
    }

    void ShowColors()
    {
        ShowColors(m_graphView, m_startNode, m_goalNode);
    }

    void ShowColors(GridVisualisation graphView, Node start, Node goal)
    {
        if (graphView == null || start == null || goal == null)
        {
            return;
        }

        //if (m_frontierNodes != null)
        //{
        //    graphView.ColorNodes(m_frontierNodes.ToList(), frontierColor);
        //}

        //if (m_exploredNodes != null)
        //{
        //    graphView.ColorNodes(m_exploredNodes, exploredColor);
        //}

        if (m_pathNodes != null && m_pathNodes.Count > 0)
        {
            graphView.ColorNodes(m_pathNodes, pathColor);
        }

        NodeVisualisation startNodeView = graphView.nodesVisualisationData[start.xIndex, start.yIndex];

        if (startNodeView != null)
        {
            startNodeView.ColorNode(startColor);
        }

        NodeVisualisation goalNodeView = graphView.nodesVisualisationData[goal.xIndex, goal.yIndex];

        if (goalNodeView != null)
        {
            goalNodeView.ColorNode(goalColor);
        }
    }

    public IEnumerator BreadthFirstSearchRoutine(float timestep = 0.1f)
    {
        float timeStart = Time.time;
        yield return null;
        while (!isComplete)
        {
            if (m_frontierNodes.Count > 0)
            {
                Node currentNode = m_frontierNodes.Dequeue();
                m_iterations++;

                if (!m_exploredNodes.Contains(currentNode))
                {
                    m_exploredNodes.Add(currentNode);
                }

                ExpandFrontier(currentNode);

                if (m_frontierNodes.Contains(m_goalNode))
                {
                    m_pathNodes = GetPathNodes(m_goalNode);
                    if (exitOnGoal)
                    {
                        
                        ShowDiagnostics();
                        isComplete = true;
                    }
                }

                //if (showIterations)
                //{
                //    ShowDiagnostics();
                //    yield return new WaitForSeconds(timestep);
                //}
            }
            else
            {
                isComplete = true;
                Debug.Log("Path is blocked, no path possible to goal");
                 yield break;
            }
        }
        Debug.Log("Pathfinder searchroutine: elapse time = " + (Time.time - timeStart).ToString() + " seconds");
    }

    //public IEnumerator DepthFirstSearchRoutine(float timestep = 0.1f)
    //{
    //    float timeStart = Time.time;
    //    yield return null;

    //    while (!isComplete)
    //    {
    //        if (m_frontNodes.Count > 0)
    //        {
    //            Node currentNode = m_frontNodes[m_frontNodes.Count - 1];
    //            m_iterations++;
    //            m_frontNodes.RemoveAt(m_frontNodes.Count - 1);
    //            if (!m_exploredNodes.Contains(currentNode))
    //            {
    //                m_exploredNodes.Add(currentNode);
    //            }

    //            DepthExpandFrontier(currentNode);

    //            if (m_frontNodes.Contains(m_goalNode))
    //            {
    //                m_pathNodes = GetPathNodes(m_goalNode);
    //                if (exitOnGoal)
    //                {
    //                    isComplete = true;
    //                }
    //            }

    //            if (showIterations)
    //            {
    //                ShowDiagnostics();
    //                yield return new WaitForSeconds(timestep);
    //            }
    //        }
    //        else
    //        {
    //            isComplete = true;
    //        }
    //    }
    //    ShowDiagnostics();
    //    Debug.Log("Pathfinder searchroutine: elapse time = " + (Time.time - timeStart).ToString() + " seconds");
    //}
    void ShowDiagnostics()
    {
        if (showColors)
        {
            ShowColors();
        }

        if (m_graphView != null && showArrows)
        {
            m_graphView.ShowNodeArrows(m_frontierNodes.ToList(), arrowColor);

            if (m_frontierNodes.Contains(m_goalNode))
            {
                m_graphView.ShowNodeArrows(m_pathNodes, highLightColor);
            }
        }
    }

    void ExpandFrontier(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbours.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbours[i]) && 
                    (!m_frontierNodes.Contains(node.neighbours[i])
                    && node.neighbours[i].nodeType != NodeType.Blocked)) 
                {
                    node.neighbours[i].previous = node;
                    m_frontierNodes.Enqueue(node.neighbours[i]);
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
        Node currentNode = endNode.previous;
        while(currentNode != null)
        {
            path.Insert(0, currentNode); //saves us haivng to reverse if we use add
            currentNode = currentNode.previous;
        }

        return path;
    }
}
