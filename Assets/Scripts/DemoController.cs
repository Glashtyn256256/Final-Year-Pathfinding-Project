using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoController : MonoBehaviour
{
    public Unit unitPrefab;
    Ray ray;
    RaycastHit hit;
    public MapData mapData;
    public Grid grid;
    GridVisualisation gridVisualisation;
    public Pathfinder pathFinder;
    public int startX = 1;
    public int startY = 4;
    public int goalX = 20;
    public int goalY = 20;
    public float timeStep = 0.1f;
    Node startNode;
    NodeVisualisation goalNode;
   public List<Unit> unitData;

    void Start()
    {
        if (mapData != null && grid != null)
        {
            int[,] mapinstance = mapData.MakeMap();
            grid.Init(mapinstance);

            gridVisualisation = grid.gameObject.GetComponent<GridVisualisation>();

            if (gridVisualisation != null)
            {
                gridVisualisation.Init(grid);
            }

            goalNode = gridVisualisation.nodesVisualisationData[goalX, goalY];
            gridVisualisation.ChangeToGoalNode(gridVisualisation.nodesVisualisationData[goalX,goalY]);
            Unit unit = Instantiate(unitPrefab, grid.nodes[startX,startY].position + unitPrefab.transform.position, Quaternion.identity) as Unit;

            unit.SetUnitPositionInWorldAndGrid(gridVisualisation.nodesVisualisationData[goalX, goalY].transform, goalX, goalY);
            unit.InitPathfinder(grid, gridVisualisation);
            unitData.Add(unit);

            unit = Instantiate(unitPrefab, grid.nodes[5, 5].position + unitPrefab.transform.position, Quaternion.identity) as Unit;

            unit.SetUnitPositionInWorldAndGrid(gridVisualisation.nodesVisualisationData[5, 5].transform, 5, 5);
            unit.InitPathfinder(grid, gridVisualisation);
            unitData.Add(unit);
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
                    if (nodeview != goalNode)
                    {
                        if (nodeview.gridNode.nodeType == 0)
                        {
                            nodeview.gridNode.nodeType = NodeType.Blocked;
                            nodeview.EnableObject(nodeview.wall, true);
                            gridVisualisation.ResetGridVisualisation();
                            for (int i = 0; i < unitData.Count; i++)
                            {
                                if (unitData[i].SearchPathChangedNode(nodeview.gridNode))
                                {
                                    
                                    unitData[i].UnitFindPath(goalNode.transform.position);
                                }
                            }
                            
                        }
                        else
                        {
                            nodeview.gridNode.nodeType = NodeType.Open;
                            nodeview.EnableObject(nodeview.wall, false);
                            gridVisualisation.ResetGridVisualisation();
                            

                        }
                    }
                }
            }
        }
        //Right click
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.gameObject.tag);
                //added mesh chollider to tile to make sure we are able to hit the tile plane.
                if (hit.collider.gameObject.tag == "Node")
                {
                    NodeVisualisation nodeview = hit.collider.gameObject.GetComponentInParent<NodeVisualisation>();
                    if (nodeview.gridNode.nodeType == NodeType.Open)
                    {
                        gridVisualisation.ResetGridVisualisation();
                        gridVisualisation.ChangeToFloorNode(gridVisualisation.nodesVisualisationData[goalNode.gridNode.xIndex, goalNode.gridNode.yIndex]);
                        gridVisualisation.ChangeToGoalNode(nodeview);
                        goalNode = nodeview;
                       
                        for (int i = 0; i < unitData.Count; i++)
                        {
                            unitData[i].UnitFindPath(goalNode.transform.position);
                        }
                        // StopCoroutine(pathFinder.BreadthFirstSearchRoutine(timeStep));
                        //gridVisualisation.ResetGridVisualisation();

                        //StartCoroutine(pathFinder.BreadthFirstSearchRoutine(timeStep));

                    }
                }
            }
        }
        
    }
}