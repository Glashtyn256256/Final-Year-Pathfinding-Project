using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridManager))]
public class GridVisualisation : MonoBehaviour
{
    public GameObject nodeVisualisationPrefab;
    public NodeVisualisation[,] nodesVisualisationData;

    public Color baseColor = Color.white;
    public Color wallColor = Color.black;
    public Color goalColor = Color.red;

    public void CreateGridVisualisation(GridManager grid)
    {
        if (grid == null)
        {
            Debug.LogWarning("GridManager! No grid");
            return;
        }

        nodesVisualisationData = new NodeVisualisation[grid.GetGridWidth, grid.GetGridHeight];

        foreach (Node node in grid.gridNodes)
        {
            GameObject instance = Instantiate(nodeVisualisationPrefab, Vector3.zero, Quaternion.identity);
            NodeVisualisation nodeVisualisation = instance.GetComponent<NodeVisualisation>();

            if (nodeVisualisation != null)
            {
                nodeVisualisation.CreateNodeVisualisation(node);
                nodesVisualisationData[node.xIndexPosition, node.yIndexPosition] = nodeVisualisation;
                if (node.nodeType == NodeType.Blocked)
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
                NodeVisualisation nodeVisualisation = nodesVisualisationData[n.xIndexPosition, n.yIndexPosition];

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
            NodeVisualisation nodeView = nodesVisualisationData[node.xIndexPosition, node.yIndexPosition];
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

    public void ChangeToGoalNodeColourOnly(NodeVisualisation nodevisualisation)
    { 
        nodevisualisation.ColorNode(goalColor);
    }
}
    
    
