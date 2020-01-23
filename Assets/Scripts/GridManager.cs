using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    //A 2d array of nodes.
    public Node[,] gridNodes;

    //int[,] mapData;
    private int gridWidth;
    private int gridHeight;
    public int GetGridWidth { get { return gridWidth; } }
    public int GetGridHeight { get { return gridHeight; } }
    //Saves us time having to use SQRT which is expensive to use.
    const float squareRouteOf2 = 1.41421356237f;
    //public int scaleDistanceOne = 10;
    //public int scaleDistanceTwo = 14;

    //Create two one for all directions and one for four directions, then have a option to toggle it on or off.
    public static readonly Vector2[] nodeNeighbourDirections =
    {   
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

    public void CreateGrid(int[,] mapdata)
    {
        gridWidth = mapdata.GetLength(0); //first array so x
        gridHeight = mapdata.GetLength(1); //first array so y;

        gridNodes = new Node[gridWidth, gridHeight];

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                //change nodetype depending on what number we gave it in the mapdata
                NodeType type = (NodeType)mapdata[x, y];
                //Node world position
                Vector3 worldPosition = new Vector3(x, 0, y);
                //create a new node
                Node newNode = new Node(x, y, type, worldPosition);
                //place it in the grid of nodes
                gridNodes[x, y] = newNode;
            }
        }

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                //pre-calc neighbours.
                gridNodes[x, y].neighbours = GetNeighbours(x, y);
            }
        }
    }

    public bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight);
    }
    List<Node> GetNeighbours(int x, int y)
    {
        return GetNeighbours(x, y, gridNodes, nodeNeighbourDirections);
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


    //Nodes world position matches the nodes grid position in the array so 3,0,3 (x,y,z) in world space is 3,3 in grid.
    //When the playernode is passed through it may be between two nodes so we round it to the closest int meaning
    //the node it is closest to. This might result in it going back to a node if it's closer.
    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x);
        int y = Mathf.RoundToInt(worldPosition.z);

        return gridNodes[x, y];
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
    public float GetNodeDistance(Node source, Node target, int heuristicindex)
    {
        switch (heuristicindex)
        {
            case 0:
                return PatricksDiagnolDistance(source, target);
            case 1:
                return ManhattenDistance(source, target);
            case 2:
                return EuclideanDistance(source, target);
            case 3:
                return ChebyshevDistance(source, target);
            case 4:
                return OctileDistance(source, target);
                
        }
        //This will never be hit but we have to return a value since it will complain
        return Mathf.Infinity;
    }

    float PatricksDiagnolDistance(Node source, Node target)
    {
        int xDistance = Mathf.Abs(source.xIndexPosition - target.xIndexPosition);
        int yDistance = Mathf.Abs(source.yIndexPosition - target.yIndexPosition);

        if (xDistance > yDistance)
        {
            return 14 * yDistance + 10 * (xDistance - yDistance);
        }
        return 14 * xDistance + 10 * (yDistance - xDistance);
    }

    //Only use this if you are using for four directions #up,down,left,right
    float ManhattenDistance(Node source, Node target)
    {
        int xDistance = Mathf.Abs(source.xIndexPosition - target.xIndexPosition);
        int yDistance = Mathf.Abs(source.yIndexPosition - target.yIndexPosition);

        return 10 * (xDistance + yDistance);
    }

    //This is a straight line distance, if your unit can move any direction then you would 
    //want to use this.
    float EuclideanDistance(Node source, Node target)
    {
        int xDistance = Mathf.Abs(source.xIndexPosition - target.xIndexPosition);
        int yDistance = Mathf.Abs(source.yIndexPosition - target.yIndexPosition);

        return 10 * Mathf.Sqrt((xDistance * xDistance) + (yDistance * yDistance));
    }

    //All distances cost the same, diagnol and cardinal movements cost 10
    float ChebyshevDistance(Node source, Node target)
    {
        int xDistance = Mathf.Abs(source.xIndexPosition - target.xIndexPosition);
        int yDistance = Mathf.Abs(source.yIndexPosition - target.yIndexPosition);
        //D = 10 and D2 = 10
        ////D * max(dx, dy) + (D2-D) * min(dx, dy)
        return 10 * Mathf.Max(xDistance, yDistance) + Mathf.Min(xDistance, yDistance);
    }

    //we use 10 and 14 in our distances instead of using 1 or 1.4 using whole numbers works.
    //D1 is 1 multiply by 10 to ge t10 and D2 is 1.4 which is squareroute of two, multiply
    //by 10 and we get 14.
    //Caridnal movement is 10 but DiagnolMovement is 14.
    float OctileDistance(Node source, Node target)
    {
        int xDistance = Mathf.Abs(source.xIndexPosition - target.xIndexPosition);
        int yDistance = Mathf.Abs(source.yIndexPosition - target.yIndexPosition);
        //D = 10 AND D2 = 14
        //1.41421356237 square route of 2 
        //D * (dx + dy) + (D2 - 2 * D) * min(dx, dy)
        return 10 * (xDistance + yDistance) + (14 - 2 * 10) * Mathf.Min(xDistance, yDistance);
    }


    public void IntegrationField()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if(gridNodes[x,y].nodeType == NodeType.Blocked)
                {
                    continue;
                }

                gridNodes[x, y].nodeParent = gridNodes[x, y];

                foreach (Node neighbour in gridNodes[x, y].neighbours)
                {
                    if (neighbour.nodeType == NodeType.Blocked)
                    {
                        continue;
                    }

                    if (neighbour.gCost < gridNodes[x, y].nodeParent.gCost)
                    {
                        gridNodes[x, y].nodeParent = neighbour;
                    }
                }
            }
        }
    }
}
