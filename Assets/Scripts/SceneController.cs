using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public MapData mapData;
    public GridManager grid;
    public Unit unitPrefab;
    public List<Unit> unitData;

    GridVisualisation gridVisualisation;
    NodeVisualisation startNode;
    NodeVisualisation goalNode;

    public InputField xGoalInput;
    public InputField yGoalInput;


    Ray ray;
    RaycastHit hit;
    
    int xStart = 1;
    int yStart = 4;
    int xGoal = 20;
    int yGoal = 20;

    int xGoalInputCanvas;
    int yGoalInputCanvas;

    int algorithmIndex;
    bool PathfindingVisualisation;

    void Start()
    {
        if (mapData != null && grid != null)
        {
            int[,] mapinstance = mapData.MakeMap();
            grid.CreateGrid(mapinstance);

            gridVisualisation = grid.gameObject.GetComponent<GridVisualisation>();

            if (gridVisualisation != null)
            {
                gridVisualisation.CreateGridVisualisation(grid);
            }

            //if our goal is not in the map range it will throw.
            if(grid.IsWithinBounds(xGoal,yGoal) && grid.IsWithinBounds(xStart, yStart))
            {
                startNode = gridVisualisation.nodesVisualisationData[xStart, yStart];
                goalNode = gridVisualisation.nodesVisualisationData[xGoal, yGoal];
                InstantiateUnit(startNode);
            }
        }
    }

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LeftMouseClickToAlterNodeTerrain();
        RightMouseClickToChangeGoalNodePositon();
        MiddleMouseClickToSpawnUnit();
    }

    //Get the intgeger value selected from the dropdown
    public void SetCurrentAlgorithmFromDropdown(int algorithmindex)
    {
        algorithmIndex = algorithmindex;
    }
    public void SetYCanvasInput(string yinput)
    {
        yGoalInputCanvas = int.Parse(yinput);
    }
    public void SetXCanvasInput(string xinput)
    {
        xGoalInputCanvas = int.Parse(xinput);
    }
    public void ChangeGoalPositionOnButtonClick()
    {
        if (xGoalInput.text != "" && yGoalInput.text != "")
        {
            xGoalInputCanvas = int.Parse(xGoalInput.text);
            yGoalInputCanvas = int.Parse(yGoalInput.text);

            if (grid.IsWithinBounds(xGoalInputCanvas, yGoalInputCanvas))
            {
                NodeVisualisation node = gridVisualisation.nodesVisualisationData[xGoalInputCanvas, yGoalInputCanvas];
                if (node.gridNode.nodeType != NodeType.Blocked)
                {
                    ChangeTileToGoalNode(node);
                    RecalculateUnitPath();
                }
                else
                {
                    Debug.Log("Can't place goal on a blocked node please input a valid walkable node");
                }
            }
            else
            {
                Debug.Log("Number Inputted is out of bounds. Please put a number that corresponds to the size of the map");
            }
        }
        else
        {
            Debug.Log("A value must be given");
        }
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
        Unit unit = Instantiate(unitPrefab, node.transform.position + unitPrefab.transform.position, Quaternion.identity);
        unit.SetUnitPositionInGrid(node.gridNode.xIndexPosition, node.gridNode.yIndexPosition);
        unit.InitiatePathfinding(grid, gridVisualisation);
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

    //When you left click the mouse button, it will
    void LeftMouseClickToAlterNodeTerrain()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.gameObject.tag);
                if (hit.collider.gameObject.tag == "Node")
                {
                    NodeVisualisation nodeVisualisation = hit.collider.gameObject.GetComponentInParent<NodeVisualisation>();
                    if (nodeVisualisation != goalNode)
                    {
                        if (nodeVisualisation.gridNode.nodeType == 0)
                        {
                            ChangeTileTerrain(nodeVisualisation, NodeType.Blocked, true);
                            SearchUnitPathRecalculate(nodeVisualisation);
                        }
                        else
                        {
                            ChangeTileTerrain(nodeVisualisation, NodeType.Open, false);
                            RecalculateUnitPath();
                            //When we put back to a floor we recalculate a path. Need a better way since it may not even effect path
                        }
                    }
                }
            }
        }
    }

    void RightMouseClickToChangeGoalNodePositon() 
    {
        //Right click
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.gameObject.tag);
                //added mesh chollider to tile to make sure we are able to hit the tile plane.
                if (hit.collider.gameObject.tag == "Node")
                {
                    NodeVisualisation nodeVisualisation = hit.collider.gameObject.GetComponentInParent<NodeVisualisation>();
                    if (nodeVisualisation.gridNode.nodeType == NodeType.Open)
                    {
                        ChangeTileToGoalNode(nodeVisualisation);
                        RecalculateUnitPath();
                    }
                }
            }
        }
    }

    void MiddleMouseClickToSpawnUnit()
    {
        if (Input.GetMouseButtonDown(2))
        {
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.gameObject.tag);
                //added mesh chollider to tile to make sure we are able to hit the tile plane.
                if (hit.collider.gameObject.tag == "Node")
                {
                    NodeVisualisation nodeVisualisation = hit.collider.gameObject.GetComponentInParent<NodeVisualisation>();
                    if (nodeVisualisation.gridNode.nodeType == NodeType.Open)
                    {
                        InstantiateUnit(nodeVisualisation);
                    }
                }
            }
        }
    }
}

