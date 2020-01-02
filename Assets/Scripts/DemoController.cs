using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoController : MonoBehaviour
{
    bool PathfindingVisualisation;
    public Unit unitPrefab;
    Ray ray;
    RaycastHit hit;
    public MapData mapData;
    public GridManager grid;
    GridVisualisation gridVisualisation;
    public Pathfinder pathFinder;
    public int startX = 1;
    public int startY = 4;
    public int goalX = 20;
    public int goalY = 20;
    public float timeStep = 0.1f;
    NodeVisualisation startNode;
    NodeVisualisation goalNode;
   public List<Unit> unitData;
    int algorithmIndex;

    void Start()
    {
        if (mapData != null && grid != null)
        {
            int[,] mapinstance = mapData.MakeMap();
            grid.CreateGrid(mapinstance);

            gridVisualisation = grid.gameObject.GetComponent<GridVisualisation>();

            if (gridVisualisation != null)
            {
                gridVisualisation.Init(grid);
            }

            //if our goal is not in the map range it will throw.
            if(grid.IsWithinBounds(goalX,goalY) && grid.IsWithinBounds(startX, startY))
            {
                startNode = gridVisualisation.nodesVisualisationData[startX, startY];
                goalNode = gridVisualisation.nodesVisualisationData[goalX, goalY];
                InstantiateUnit(startNode);
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
                    if (nodeview != goalNode)
                    {
                        if (nodeview.gridNode.nodeType == 0)
                        {
                            ChangeTileTerrain(nodeview, NodeType.Blocked, true);
                            SearchUnitPathRecalculate(nodeview);     
                        }
                        else
                        {
                            ChangeTileTerrain(nodeview, NodeType.Open, false);
                            RecalculateUnitPath();
                            //When we put back to a floor we recalculate a path. Need a better way since it may not even effect path
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
                        ChangeTileToGoalNode(nodeview);
                        RecalculateUnitPath();
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(2))
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
                        InstantiateUnit(nodeview);
                    }
                }
            }
        }

    }

    //Get the intgeger value selected from the dropdown
    public void SetCurrentAlgorithmFromDropdown(int algorithmindex)
    {
        algorithmIndex = algorithmindex;
    }

    void ChangeTileToGoalNode(NodeVisualisation node)
    {
        gridVisualisation.ResetGridVisualisation();
        gridVisualisation.ChangeToFloorNode(goalNode);
        gridVisualisation.ChangeToGoalNode(node);
        goalNode = node;
    }

    void ChangeTileTerrain(NodeVisualisation node, NodeType nodeType, bool wallStatus)
    {
        node.gridNode.nodeType = nodeType;
        node.EnableObject(node.wall, wallStatus);
        gridVisualisation.ResetGridVisualisation();
    }
    
    void InstantiateUnit(NodeVisualisation node)
    {
        Unit unit = Instantiate(unitPrefab, node.transform.position + unitPrefab.transform.position, Quaternion.identity) as Unit;
        unit.SetUnitPositionInWorldAndGrid(node.gridNode.xIndexPosition, node.gridNode.yIndexPosition);
        unit.InitPathfinder(grid, gridVisualisation);
        unitData.Add(unit);
        unit.UnitFindPath(goalNode.transform, algorithmIndex);
        unit.UnitPathVisualisation();
        gridVisualisation.ChangeToGoalNodeColourOnly(goalNode);
    }

    void RecalculateUnitPath() 
    {
        for (int i = 0; i < unitData.Count; i++)
        {
            unitData[i].UnitFindPath(goalNode.transform, algorithmIndex);
            unitData[i].UnitPathVisualisation();
            gridVisualisation.ChangeToGoalNodeColourOnly(goalNode);
        }
    }

    void SearchUnitPathRecalculate(NodeVisualisation node)
    {
        for (int i = 0; i < unitData.Count; i++)
        {
            if (unitData[i].SearchPathChangedNode(node.gridNode))
            {
                unitData[i].UnitFindPath(goalNode.transform, algorithmIndex);
            }
            unitData[i].UnitPathVisualisation();
            gridVisualisation.ChangeToGoalNodeColourOnly(goalNode);
        }
    }
}