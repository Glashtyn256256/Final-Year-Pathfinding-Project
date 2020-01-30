using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowfieldUnit : MonoBehaviour
{
    float unitSpeed = 0.1f;
    float unitWeight = 50.0f;
    int xSpawnPosition;
    int ySpawnPosition;

    bool reachedTarget;
    GridManager gridFlowfield;
   

   public void InstaniateUnit(int xspawnposition, int yspawnposition, GridManager grid, int indexposition)
    {
        reachedTarget = true;
        //Where the unit was sppawned on the grid. Will be used so when we reset the map we can move the unit back to original position.
        xSpawnPosition = xspawnposition;
        ySpawnPosition = yspawnposition;
        gridFlowfield = grid;
        this.name = "Unit " + indexposition; 
    }

    IEnumerator MoveUnitToNode(Node currentnode)
    {
        bool UnitPastCurrentNode = false;
        currentnode.UnitAbove = true;

        Vector3 tempNodePos = new Vector3(currentnode.nodeParent.nodeWorldPosition.x,
            transform.position.y, currentnode.nodeParent.nodeWorldPosition.z);
        

        //gridFlowfield.RecalculateNeighbours(currentnode);
        //issue it goes down in y :/
        //Stop it sinking into the ground, need a better way of fixing this.
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position ,tempNodePos, unitSpeed);
            if(!UnitPastCurrentNode)
            {
                if(gridFlowfield.CheckIfUnitOnNode(transform.position, currentnode))
                {
                    UnitPastCurrentNode = true;
                }
            }

            if (transform.position == tempNodePos)
            {
                SetReachedTarget(true);
                yield break;
            } 

            yield return null;
        }
    }

    public void ResetUnitPositionBackToOriginal()
    {
       // StopCoroutine("MoveUnitAcrossPath");
        //yspawnposition is meant to be in z, confusing name.
        transform.position = new Vector3(xSpawnPosition, transform.position.y, ySpawnPosition);
    }

    public void ChangeUnitPositionWithoutUsingSpawnPosition(int x, int y)
    {
       // StopCoroutine("MoveUnitAcrossPath");
        //yspawnposition is meant to be in z, confusing name.
        transform.position = new Vector3(x, transform.position.y, y);
    }

    public bool HasReachedTarget()
    {
        return reachedTarget;
    }
    public void SetReachedTarget(bool value)
    {
        reachedTarget = value;
    }
    public void UnitMovementStart(Node node)
    {
        StartCoroutine("MoveUnitToNode", node);
    }

    public void UnitMovementStop()
    {
        StopCoroutine("MoveUnitToNode");
    }
}
