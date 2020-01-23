using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeVisualisation : MonoBehaviour
{
    public GameObject tile;
    public GameObject wall;
    public GameObject arrow;

    public Node gridNode;

    [Range(0, 0.5f)]
    public float borderSize = 0.0f;

    public void CreateNodeVisualisation(Node node){

        if (tile != null && wall != null)
        {
            gameObject.name = "Node (" + node.xIndexPosition + "," + node.yIndexPosition + ")";
            gameObject.transform.position = node.nodeWorldPosition;
            tile.transform.localScale = new Vector3(1f - borderSize, 1f, 1f - borderSize);
            gridNode = node;

            if (node.nodeType == NodeType.Blocked)
            {
                wall.SetActive(true);
            }
        }
    }

    void ColorNode(Color color, GameObject go)
    {
        if (go != null)
        {
            Renderer goRenderer = go.GetComponent<Renderer>();

            if (goRenderer != null)
            {
                goRenderer.material.color = color;
            }
        }
    }

    public void ColorNode(Color color)
    {
        ColorNode(color, tile);
    }

   public void EnableObject(GameObject go, bool state)
    {
        go.SetActive(state);
    }

    public void ArrowPosition()
    {
        if (gridNode.nodeType != NodeType.Blocked)
        {
            Vector3 directionToParent = (gridNode.nodeParent.nodeWorldPosition - gridNode.nodeWorldPosition).normalized;
            arrow.transform.rotation = Quaternion.LookRotation(directionToParent);
        }
    }
}
