using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Pathfinding pathFinding;
    Transform goalWorldPosition;
    List<Node> path;

    float unitSpeed = 0.1f;

    int xSpawnPosition;
    int ySpawnPosition;

    int IndexInPath;
   



   public void InitiatePathfinding(GridManager grid, GridVisualisation gridvisualisation)
    {
        pathFinding.CreatePathfinding(grid, gridvisualisation);
    }
   public void SetUnitPositionInGrid(int xspawnposition, int yspawnposition)
    {
        //Where the unit was sppawned on the grid. Will be used so when we reset the map we can move the unit back to original position.
        xSpawnPosition = xspawnposition;
        ySpawnPosition = yspawnposition;
    }

    //Do we want to pass ina node or pass in a vector
    //Pass in type of search we're doing through unitfindpath. Then it will pick the correct one.
    public void UnitFindPath(Transform goalposition, int algorithmindex)
    {
        goalWorldPosition = goalposition;
        if (goalposition != null)
        {
            StopCoroutine("MoveUnitAcrossPath");
            path = pathFinding.FindPath(transform.position, goalWorldPosition.position, algorithmindex);
            if (path != null)
            {
                //StartCoroutine(MoveUnitAcrossPath(algorithmindex));
                StartCoroutine("MoveUnitAcrossPath",algorithmindex);
            }
        }
    }
    IEnumerator MoveUnitAcrossPath(int algorithmindex)
    {
        Vector3 currentNodeWorldPosition = path[0].nodeWorldPosition;
        IndexInPath = 0;

        while (true)
        {
            if (transform.position == currentNodeWorldPosition)
            {
                IndexInPath++;
                if (IndexInPath > path.Count - 1)
                {
                    yield break;
                }
                currentNodeWorldPosition = path[IndexInPath].nodeWorldPosition;
            }

            //Need a way to check the path after this to see if it's blocked, otherwise unit may be put inside wall.
            if (path[IndexInPath].nodeType == NodeType.Blocked)
            {
                path = pathFinding.FindPath(transform.position, goalWorldPosition.position, algorithmindex);
                IndexInPath = 0;
                yield return null;
            }
            //issue it goes down in y :/
            //Stop it sinking into the ground, need a better way of fixing this.
            currentNodeWorldPosition.y = transform.position.y;
            transform.position = Vector3.MoveTowards(transform.position, currentNodeWorldPosition, unitSpeed);
            yield return null;
        }
    }

        //Instead of recalculating a path every time a node is turned into a wall. We check
        //the units current path and if the changednode is in that path we re-calculate a new path.
        //This won't work when we turn a wall into a node.
    public bool SearchPathChangedNode(Node changedNode)
    {
        if (path != null)
        {
            foreach (var node in path)
            {
                if (node == changedNode)
                {
                    return true;
                }
            }
        }
        return false;
    }

   public void UnitPathVisualisation()
    {
        pathFinding.ShowColors();
    }

    public void ResetUnitPositionBackToOriginal()
    {
        StopCoroutine("MoveUnitAcrossPath");
        //yspawnposition is meant to be in z, confusing name.
        transform.position = new Vector3(xSpawnPosition, transform.position.y, ySpawnPosition);
    }

    public void ChangeUnitPositionWithoutUsingSpawnPosition(int x, int y)
    {
        StopCoroutine("MoveUnitAcrossPath");
        //yspawnposition is meant to be in z, confusing name.
        transform.position = new Vector3(x, transform.position.y, y);
    }
}
