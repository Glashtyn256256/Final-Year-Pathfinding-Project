using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
   
    public Node[,] nodes;
    public List<Node> walls = new List<Node>();

    int[,] m_mapData;
    int m_width;
    public int Width { get { return m_width; } }
    int m_height;
    public int Height { get{ return m_height; } }

    public static readonly Vector2[] allDirections =
    {   //x, y 
        new Vector2(0f,1f), //top
        new Vector2(1f,1f), //top right
        new Vector2(1f,0f), //right
        new Vector2(1f,-1f), //bottom right
        new Vector2(0f,-1f), //bottom
        new Vector2(-1f,-1f), //bottom left
        new Vector2(-1f,0f), // left
        new Vector2(-1f,1f), // top left
    };

    public void Init(int[,] mapData)
    {
        m_mapData = mapData;
        m_width = mapData.GetLength(0); //first array so x
        m_height = mapData.GetLength(1); //first array so y;

        nodes = new Node[m_width, m_height];

        for (int y = 0; y < m_height; y++)
        {
            for (int x = 0; x < m_width; x++)
            {
                //change nodetype depending on what number we gave it in the mapdata
                NodeType type = (NodeType)mapData[x, y];
                //create a new node
                Node newNode = new Node(x, y, type);
                //place it in the grid of nodes
                nodes[x, y] = newNode;

                newNode.position = new Vector3(x, 0, y);
            }
        }

        for (int y = 0; y < m_height; y++)
        {
            for (int x = 0; x < m_width; x++)
            {
  
                    nodes[x, y].neighbours = GetNeighbours(x, y);
                
            }
        }
    }

    public bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < m_width && y >= 0 && y < m_height);
    }

    List<Node> GetNeighbours(int x, int y, Node[,] nodeArray, Vector2[] directions)
    {
        List<Node> neighbourNodes = new List<Node>();

        foreach(Vector2 dir in directions)
        {
            int newX = x + (int)dir.x;
            int newY = y + (int)dir.y;

            //if(IsWithinBounds(newX,newY) && 
            //   nodeArray[newX,newY] != null &&
            //    nodeArray[newX,newY].nodeType != NodeType.Blocked)            

            //We want the blocked nodes incase they've been changed so we still want to know about them.
            if (IsWithinBounds(newX, newY) && nodeArray[newX, newY] != null)
            {
                neighbourNodes.Add(nodeArray[newX, newY]);
            }
        }
        return neighbourNodes;
    }

    List<Node> GetNeighbours(int x, int y)
    {
        return GetNeighbours(x, y, nodes, allDirections);
    }
}
