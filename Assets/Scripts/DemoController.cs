using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoController : MonoBehaviour
{
    public MapData mapData;
    public Grid grid;
    public Pathfinder pathFinder;
    public int startX = 1;
    public int startY = 4;
    public int goalX = 34;
    public int goalY = 7;
    public float timeStep = 0.1f;

    void Start()
    {
        if (mapData != null && grid != null)
        {
            int[,] mapinstance = mapData.MakeMap();
            grid.Init(mapinstance);

            GraphView graphView = grid.gameObject.GetComponent<GraphView>();

            if (graphView !=null)
            {
                graphView.Init(grid);
            }

            if (grid.IsWithinBounds(startX,startY) && grid.IsWithinBounds(goalX, goalY) && pathFinder !=null)
            {
                //Node startNode = grid.nodes[startX, startY];
                //Node goalNode = grid.nodes[goalX, goalY];
                //pathFinder.Init(grid, graphView, startNode, goalNode);
                //StartCoroutine(pathFinder.BreadthFirstSearchRoutine(timeStep));
            }
        }

    }
}
