using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FlowfieldSceneController : MonoBehaviour
{
    public MapData mapData;
    public GridManager grid;
    public FlowfieldUnit unitPrefab;
    public FlowfieldPathfinding flowfieldPathfinding;
    public List<FlowfieldUnit> unitData;
    

    GridVisualisation gridVisualisation;
    NodeVisualisation startNode;
    NodeVisualisation goalNode;

    public InputField xGoalInput;
    public InputField yGoalInput;

    public InputField xUnitInput;
    public InputField yUnitInput;


    Ray ray;
    RaycastHit hit;
    
    int xStart = 1;
    int yStart = 4;
    int xGoal = 20;
    int yGoal = 20;
    int xGoalInputValue;
    int yGoalInputValue;
    int xUnitInputValue;
    int yUnitInputValue;

    int algorithmIndex;
    int terrainIndex;

    bool PathfindingVisualisationAid;

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
            flowfieldPathfinding.CreatePathfinding(grid, gridVisualisation);
            flowfieldPathfinding.FlowfieldPath(goalNode.transform.position, PathfindingVisualisationAid);
        }
    }

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LeftMouseClickToAlterNodeTerrain();
        RightMouseClickToChangeGoalNodePositon();
        MiddleMouseClickToSpawnUnit();
        UnitMovement();
    }

    //Get the intgeger value selected from the dropdown
    public void SetCurrentAlgorithmFromDropdown(int algorithmindex)
    {
        algorithmIndex = algorithmindex;
    }
    public void SetTerrainTypeFromDropdown(int terrainindex)
    {
        terrainIndex = terrainindex;
    }

    public void VisualAidShowNodeArrows(bool visualaid)
    {
        PathfindingVisualisationAid = visualaid;
        gridVisualisation.ShowNodeArrows(visualaid);
    }

    public void ToggleVisualisationAid(bool toggle)
    {
        PathfindingVisualisationAid = toggle;

        if (PathfindingVisualisationAid)
        {
            //shouldn't need to do this but just incase
            gridVisualisation.ResetGridVisualisation();
            gridVisualisation.ChangeToGoalNodeColourOnly(goalNode);         
        }
        else
        {
            gridVisualisation.ResetGridVisualisation();
            gridVisualisation.ChangeToGoalNodeColourOnly(goalNode);
        }
    }
    public void ChangeGoalPositionOnButtonClick()
    {
        //If the text box is not blank then crack on otherwise throw debug message.
        if (xGoalInput.text != "" && yGoalInput.text != "")
        {
            xGoalInputValue = int.Parse(xGoalInput.text);
            yGoalInputValue = int.Parse(yGoalInput.text);

            if (grid.IsWithinBounds(xGoalInputValue, yGoalInputValue))
            {
                NodeVisualisation node = gridVisualisation.nodesVisualisationData[xGoalInputValue, yGoalInputValue];
                if (node.gridNode.nodeType != NodeType.Blocked)
                {
                    ChangeTileToGoalNode(node);
            
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

    public void ChangeUnitPositionOnButtonClick()
    {
        
        if (xUnitInput.text != "" && yUnitInput.text != "")
        {
            xUnitInputValue = int.Parse(xUnitInput.text);
            yUnitInputValue = int.Parse(yUnitInput.text);
           
            if (grid.IsWithinBounds(xUnitInputValue, yUnitInputValue))
            {
                NodeVisualisation node = gridVisualisation.nodesVisualisationData[xUnitInputValue, yUnitInputValue];
                if (node.gridNode.nodeType != NodeType.Blocked)
                {                
                    unitData[0].ChangeUnitPositionWithoutUsingSpawnPosition(node.gridNode.xIndexPosition, node.gridNode.yIndexPosition);
                    gridVisualisation.ResetGridVisualisation();
                    gridVisualisation.ChangeToGoalNodeColourOnly(goalNode);
                }
                else
                {
                    Debug.Log("Can't place unit on a blocked node please input a valid walkable node");
                }
            }
            else
            {
                Debug.Log("Number Inputted is out of bounds. Please put a number that corresponds to the size of the map");
            }
        }
        else
        {
            Debug.Log("A value must be given for the unit");
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
        node.EnableObject(node.arrow, PathfindingVisualisationAid);
        gridVisualisation.ResetGridVisualisation();
    }
    
    void InstantiateUnit(NodeVisualisation node)
    {
        FlowfieldUnit unit = Instantiate(unitPrefab, node.transform.position + unitPrefab.transform.position, Quaternion.identity);
        unit.InstaniateUnit(node.gridNode.xIndexPosition, node.gridNode.yIndexPosition);
        unitData.Add(unit);       
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
                        
                            switch (terrainIndex)
                            {
                                case 0:
                                if (nodeVisualisation.gridNode.nodeType != NodeType.Open)
                                {
                                    ChangeTileTerrain(nodeVisualisation, NodeType.Open, false);
                                    UpdateMap();
                                }
                                    break;
                                case 1:
                                if (nodeVisualisation.gridNode.nodeType != NodeType.Blocked)
                                {
                                    ChangeTileTerrain(nodeVisualisation, NodeType.Blocked, true);
                                    UpdateMap();
                                }
                                    break;
                                case 2:
                                if (nodeVisualisation.gridNode.nodeType != NodeType.Grass)
                                {
                                    ChangeTileTerrain(nodeVisualisation, NodeType.Grass, false);
                                    UpdateMap();
                                }
                                break;
                                case 3:
                                if (nodeVisualisation.gridNode.nodeType != NodeType.Water)
                                {
                                    ChangeTileTerrain(nodeVisualisation, NodeType.Water, false);
                                    UpdateMap();
                                }
                                break;
                            }
                    }
                }
            }
        }
    }

    void ResetUnitHasReachedTarget()
    {
        foreach(var unit in unitData)
        {
            unit.UnitMovementStop();
            unit.SetReachedTarget(true);
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
                    if (nodeVisualisation.gridNode.nodeType != NodeType.Blocked)
                    {
                        ChangeTileToGoalNode(nodeVisualisation);
                        UpdateMap();
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

    void UnitMovement()
    {
        foreach(var unit in unitData) 
        {
            if (unit.HasReachedTarget())
            {
                Node currentNode = grid.GetNodeFromWorldPoint(unit.transform.position);
                if (currentNode.nodeParent.nodeType != NodeType.Blocked || currentNode.nodeType != NodeType.GoalNode)
                {
                    unit.UnitMovementStart(currentNode.nodeParent);
                    unit.SetReachedTarget(false);
                }
            }
        
        }
    }

    void UpdateMap()
    {
        flowfieldPathfinding.FlowfieldPath(goalNode.transform.position, PathfindingVisualisationAid);
        ResetUnitHasReachedTarget();
    }
}

