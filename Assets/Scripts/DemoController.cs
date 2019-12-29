using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoController : MonoBehaviour
{

    Ray ray;
    RaycastHit hit;
    public MapData mapData;
    public Grid grid;
    GridVisualisation gridVisualisation;
    public Pathfinder pathFinder;
    public int startX = 1;
    public int startY = 4;
    public int goalX = 34;
    public int goalY = 7;
    public float timeStep = 0.1f;
    Node startNode;
    Node goalNode;

    void Start()
    {
        if (mapData != null && grid != null)
        {
            int[,] mapinstance = mapData.MakeMap();
            grid.Init(mapinstance);

            gridVisualisation = grid.gameObject.GetComponent<GridVisualisation>();

            if (gridVisualisation !=null)
            {
                gridVisualisation.Init(grid);
            }

            if (grid.IsWithinBounds(startX,startY) && grid.IsWithinBounds(goalX, goalY) && pathFinder !=null)
            {
                startNode = grid.nodes[startX, startY];
                goalNode = grid.nodes[goalX, goalY];
                pathFinder.Init(grid, gridVisualisation, startNode, goalNode);
                StartCoroutine(pathFinder.BreadthFirstSearchRoutine(timeStep));
            }
        }

    }

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.gameObject.tag);
                if (hit.collider.gameObject.tag == "Node")
                {
                    NodeVisualisation nodeview = hit.collider.gameObject.GetComponentInParent<NodeVisualisation>();
                    if (nodeview.gridNode.nodeType == 0)
                    {
                        nodeview.gridNode.nodeType = NodeType.Blocked;
                        nodeview.EnableObject(nodeview.wall, true);

                       
                        StopCoroutine(pathFinder.BreadthFirstSearchRoutine(timeStep));
                        gridVisualisation.ResetGridVisualisation();
                        pathFinder.Init(grid, gridVisualisation, startNode, goalNode);
                        StartCoroutine(pathFinder.BreadthFirstSearchRoutine(timeStep));
                    }
                    else
                    {
                        nodeview.gridNode.nodeType = NodeType.Open;
                        nodeview.EnableObject(nodeview.wall, false);
                        
                        StopCoroutine(pathFinder.BreadthFirstSearchRoutine(timeStep));
                        gridVisualisation.ResetGridVisualisation();
                        pathFinder.Init(grid, gridVisualisation, startNode, goalNode);
                        StartCoroutine(pathFinder.BreadthFirstSearchRoutine(timeStep));
                    }
                }
            }
        }
    }
}
