using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeVisualisation : MonoBehaviour
{
    public GameObject tile;
    //public GameObject arrow;
     public GameObject wall;

    public Node gridNode;

    [Range(0, 0.5f)]
    public float borderSize = 0.15f;

    public void Init(Node node){

        if (tile != null)
        {
            gameObject.name = "Node (" + node.xIndexPosition + "," + node.yIndexPosition + ")";
            gameObject.transform.position = node.nodeWorldPosition;
            tile.transform.localScale = new Vector3(1f - borderSize, 1f, 1f - borderSize);
            gridNode = node;

            // EnableObject(arrow, false);
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

    public void ShowArrow(Color color)
    {
        //if (m_node != null && arrow != null && m_node.previous != null)
        //{
        //    EnableObject(arrow, true);

        //    Vector3 directionToPrevious = (m_node.previous.position - m_node.position).normalized;
        //    arrow.transform.rotation = Quaternion.LookRotation(directionToPrevious);

        //    Renderer arrowRender = arrow.GetComponent<Renderer>();
        //    if (arrowRender != null)
        //    {
        //        arrowRender.material.color = color;
        //    }
        //}
    }
}
