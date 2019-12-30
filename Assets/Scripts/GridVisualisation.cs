using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GridVisualisation : MonoBehaviour
{
    public GameObject nodeVisualisationPrefab;
    public NodeVisualisation[,] nodesVisualisationData;

    public Color baseColor = Color.white;
    public Color wallColor = Color.black;
    public Color goalColor = Color.red;

    public void Init(Grid graph)
    {
        if (graph == null)
        {
            Debug.LogWarning("Graphview! No graph");
            return;
        }

        nodesVisualisationData = new NodeVisualisation[graph.Width, graph.Height];

        foreach (Node n in graph.nodes)
        {
            GameObject instance = Instantiate(nodeVisualisationPrefab, Vector3.zero, Quaternion.identity);
            NodeVisualisation nodeVisualisation = instance.GetComponent<NodeVisualisation>();

            if (nodeVisualisation != null)
            {
                nodeVisualisation.Init(n);
                nodesVisualisationData[n.xIndex, n.yIndex] = nodeVisualisation;
                if (n.nodeType == NodeType.Blocked)
                {
                    nodeVisualisation.ColorNode(wallColor);
                }
                else 
                {
                    nodeVisualisation.ColorNode(baseColor);
                }
            }
        }
    }

    public void ColorNodes(List<Node> nodes, Color color)
    {
        foreach (Node n in nodes)
        {
            if (n != null)
            {
                NodeVisualisation nodeVisualisation = nodesVisualisationData[n.xIndex, n.yIndex];

                if (nodeVisualisation != null)
                {
                    nodeVisualisation.ColorNode(color);
                }
            }
        }
    }

    public void ShowNodeArrows(Node node, Color color)
    {
        if (node != null)
        {
            NodeVisualisation nodeView = nodesVisualisationData[node.xIndex, node.yIndex];
            if (nodeView != null)
            {
                nodeView.ShowArrow(color);
            }
        }
    }

    public void ShowNodeArrows(List<Node> nodes, Color color)
    {
        foreach (Node n in nodes)
        {
            ShowNodeArrows(n, color);
        }
    }

    public void ResetGridVisualisation()
    {
        foreach (NodeVisualisation nodeVisualisation in nodesVisualisationData)

            if (nodeVisualisation.gridNode.nodeType == NodeType.Blocked)
            {
                nodeVisualisation.ColorNode(wallColor);
            }
            else if(nodeVisualisation.gridNode.nodeType == NodeType.Open) 
            {
                nodeVisualisation.ColorNode(baseColor);
            }
            else
            {
                nodeVisualisation.ColorNode(goalColor);
            }
        
    }

    public void ChangeToFloorNode(NodeVisualisation nodevisualisation)
    {
        nodevisualisation.gridNode.nodeType = NodeType.Open;
        nodevisualisation.ColorNode(baseColor);
    }
    public void ChangeToGoalNode(NodeVisualisation nodevisualisation)
    {
        nodevisualisation.gridNode.nodeType = NodeType.GoalNode;
        nodevisualisation.ColorNode(goalColor);
    }
}
    
    
