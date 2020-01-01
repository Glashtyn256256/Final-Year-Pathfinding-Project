using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //A 2d array of nodes.
    public Node[,] nodes;

    int[,] mapData;
   private int gridWidth;
   private int gridHeight;

    public int Width { get { return gridWidth; } }
    public int Height { get { return gridHeight; } }


    public static readonly Vector2[] allDirections =
    {   //Create two one for all directions and one for four directions, then have a option to toggle it on or off.
        //x, y
        new Vector2(0f,1f), //top
        new Vector2(1f,0f), //right
        new Vector2(0f,-1f), //bottom
        new Vector2(-1f,0f), // left
        new Vector2(1f,1f), //top right
        new Vector2(1f,-1f), //bottom right    
        new Vector2(-1f,-1f), //bottom left 
        new Vector2(-1f,1f), // top left
    };

    public void Init(int[,] mapdata)
    {
        mapData = mapdata;
        gridWidth = mapdata.GetLength(0); //first array so x
        gridHeight = mapdata.GetLength(1); //first array so y;

        nodes = new Node[gridWidth, gridHeight];

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                //change nodetype depending on what number we gave it in the mapdata
                NodeType type = (NodeType)mapdata[x, y];
                //create a new node
                Node newNode = new Node(x, y, type);
                //place it in the grid of nodes
                nodes[x, y] = newNode;

                //Position in world space
                newNode.nodePosition = new Vector3(x, 0, y);
            }
        }

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                //pre-calc neighbours.
                nodes[x, y].neighbours = GetNeighbours(x, y);
            }
        }
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

            //We want the blocked nodes incase they've been changed so we still want to know about them.
            //Make sure we dont add nodes that are outside the boundrys of the grid
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
        //Nodes world position matches the nodes grid position in the array so 3,0,3 (x,y,z) in world space is 3,3 in grid.
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
     //Issue with previous herustic, noticed it wasnt getting the most optimal path
     //May be wrong but I feel it finds it faster for a less optimal path
     //changed it and now getting optimal path. 
    public int GetNodeDistance(Node source, Node target)
    {
        int distX = Mathf.Abs(source.xIndexPosition - target.xIndexPosition);
        int distY = Mathf.Abs(source.yIndexPosition - target.yIndexPosition);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }

}
