using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{

    public Node[,] nodes;
    public List<Node> walls = new List<Node>();

    int[,] m_mapData;
    int gridWidth;
    public int Width { get { return gridWidth; } }
    int gridHeight;
    public int Height { get { return gridHeight; } }
    float nodeDiameter;
    float gridSizeX;
    float gridSizeY;

    public static readonly Vector2[] allDirections =
    {   //Create two one for all directions and one for four directions, then have a option to toggle it on or off.
        //x, y
        new Vector2(0f,1f), //top
        new Vector2(1f,0f), //right
        new Vector2(0f,-1f), //bottom
        new Vector2(-1f,0f), // left
        //new Vector2(1f,1f), //top right
        //new Vector2(1f,-1f), //bottom right    
        //new Vector2(-1f,-1f), //bottom left 
        //new Vector2(-1f,1f), // top left
    };

    public void Init(int[,] mapData)
    {
        m_mapData = mapData;
        gridWidth = mapData.GetLength(0); //first array so x
        gridHeight = mapData.GetLength(1); //first array so y;

        nodes = new Node[gridWidth, gridHeight];

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
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

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                nodes[x, y].neighbours = GetNeighbours(x, y);
            }
        }

        //nodeDiameter = 1 * 2;//0.70710678118f * 2;
        //gridSizeX = Mathf.RoundToInt(gridWidth / nodeDiameter); //How many nodes we can fit into our worldsize X 
        //gridSizeY = Mathf.RoundToInt(gridHeight / nodeDiameter);
    }

    public bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight);
    }

    List<Node> GetNeighbours(int x, int y, Node[,] nodeArray, Vector2[] directions)
    {
        List<Node> neighbourNodes = new List<Node>();

        foreach (Vector2 dir in directions)
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

    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        //float percentX = (worldPosition.x - transform.position.x) / gridWorldSize.x + 0.5f - (nodeRadius / gridWorldSize.x);
        //float percentY = (worldPosition.z - transform.position.z) / gridWorldSize.y + 0.5f - (nodeRadius / gridWorldSize.y);
        //float percentX = (worldPosition.x - transform.position.x) / gridWidth + 0.5f;
        //float percentY = (worldPosition.z - transform.position.z) / gridHeight + 0.5f;
        // float percentX = (worldPosition.x + gridWidth / 2) / gridWidth;
        //   float percentY = (worldPosition.z + gridHeight / 2) / gridHeight;
        // percentX = Mathf.Clamp01(percentX);
        // percentY = Mathf.Clamp01(percentY);

        //int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        //int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        //Nodes world position matches the nodes grid position in the array so 3,3 in world space is 3,3 in grid.
        //When the playernode is passed through it may be between two nodes so we round it to the closest int meaning
        //the node it is closest to. This might result in it going back to a node if it's closer. 
        float percentX = worldPosition.x;
        float percentY = worldPosition.z;
        
        int x = Mathf.RoundToInt(percentX);
        int y = Mathf.RoundToInt(percentY);

        return nodes[x, y];
    }
    //void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireCube(nodes[0, 0].position, new Vector3(gridWidth, 1, gridHeight)); //Reason we use Y axis and not z is because the grid

    //    if (nodes != null)
    //    {
    //        foreach (Node n in nodes)
    //        {

    //            Gizmos.DrawCube(n.position, Vector3.one * (nodeDiameter));
    //        }
    //    }
    //}

     //Talk about this more later, from tutorial. explain in write up what it does.
    public int GetNodeDistance(Node source, Node target)
    {
        int dx = Mathf.Abs(source.xIndex - target.xIndex);
        int dy = Mathf.Abs(source.yIndex - target.yIndex);

        int min = Mathf.Min(dx, dy);
        int max = Mathf.Max(dx, dy);

        int diagonalSteps = min;
        int straightSteps = max - min;

        return (14 * diagonalSteps + straightSteps);
    }
}
