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
            ShowNodeArrows(n,color);
        }
    }

    public void ResetGridVisualisation()
    {
        foreach(NodeVisualisation nodeview in nodesVisualisationData)

        if (nodeview.gridNode.nodeType == NodeType.Blocked)
        {
               nodeview.ColorNode(wallColor);
        }
        else
        {
            nodeview.ColorNode(baseColor);
        }
    }
}
