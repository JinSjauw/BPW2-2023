using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Pathfinding
{
    //CONST
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private LevelGrid grid;
    private List<GridObject> nodeGrid;
    private List<GridObject> openList;
    private List<GridObject> closedList;

    public Pathfinding()
    {
        if (LevelGrid.Instance == null)
        {
            Debug.Log("ERROR: No level grid");
            return;
        }
        
        grid = LevelGrid.Instance;
        nodeGrid = grid.GetNodeGrid();

    }

    /*public List<GridObject> FindPath(GridObject _start, GridObject _end)
    {
        GridObject startNode = _start;
        GridObject endNode = _end;

        openList = new List<GridObject> { startNode };
        closedList = new List<GridObject>();

        //Reset Nodes
        foreach (var node in nodeGrid)
        {
            node.gCost = int.MaxValue;
            node.CalculateFCost();
            node.parent = null;
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistance(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            GridObject currentNode = GetLowestFcost(openList);

            if (currentNode == endNode)
            {
                //Return path here. End has been reached
                return ReturnPath(startNode, endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbour in currentNode.neighbourList)
            {
                if (closedList.Contains(neighbour))
                {
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistance(currentNode, neighbour);
                if (tentativeGCost < neighbour.gCost)
                {
                    neighbour.parent = currentNode;
                    neighbour.gCost = tentativeGCost;
                    neighbour.hCost = CalculateDistance(neighbour, endNode);
                    neighbour.CalculateFCost();

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }
        
        return null;
    }*/

    public List<Vector3> GetPath(GridPosition _start, GridPosition _end)
    {
        GridObject start = grid.GetGridObject(_start);
        GridObject end = grid.GetGridObject(_end);

        List<GridPosition> foundPath = FindPath(start, end);
        List<Vector3> path = new List<Vector3>();

        foreach (GridPosition position in foundPath)
        {
            path.Add(grid.GetWorldPosition(position));
        }

        return path;
    }

    public List<GridPosition> FindPath(GridObject _start, GridObject _end)
    {
        GridObject startNode = _start;
        GridObject endNode = _end;

        openList = new List<GridObject> { startNode };
        closedList = new List<GridObject>();

        //Reset Nodes
        foreach (var node in nodeGrid)
        {
            node.gCost = int.MaxValue;
            node.CalculateFCost();
            node.parent = null;
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistance(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            GridObject currentNode = GetLowestFcost(openList);

            if (currentNode == endNode)
            {
                //Return path here. End has been reached
                return ReturnPath(startNode, endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbour in currentNode.neighbourList)
            {
                if (closedList.Contains(neighbour))
                {
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistance(currentNode, neighbour);
                if (tentativeGCost < neighbour.gCost)
                {
                    neighbour.parent = currentNode;
                    neighbour.gCost = tentativeGCost;
                    neighbour.hCost = CalculateDistance(neighbour, endNode);
                    neighbour.CalculateFCost();

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }
        
        return null;
    }
    
    public List<GridPosition> ReturnPath(GridObject _startNode, GridObject _endNode)
    {
        List<GridPosition> path = new List<GridPosition>();
        GridObject currentNode = _endNode;
        path.Add(currentNode.gridPosition);
        while (currentNode != _startNode)
        {
            path.Add(currentNode.parent.gridPosition);
            currentNode = currentNode.parent;
        }
        
        path.Reverse();
        
        return path;
    }

    private int CalculateDistance(GridObject _a, GridObject _b)
    {
        if(_a == null || _b == null)
        {
            Debug.Log("Pathnode missing");
            return 0;
        }

        int distanceX = (int)MathF.Abs(_a.position.x - _b.position.x);
        int distanceY = (int)MathF.Abs(_a.position.z - _b.position.z);
        
        /*
        int remaining = Mathf.Abs(distanceX - distanceY);
        
        int score = MOVE_DIAGONAL_COST * Mathf.Min(distanceX, distanceY) + MOVE_STRAIGHT_COST * remaining;
        */
        
        int score = 4 * (distanceX + distanceY);

        if (_a.tileType == TILETYPE.FLOOR)
        {
            score += 1;
        }

        if (_a.tileType == TILETYPE.HALLWAY)
        {
            score -= 3;
        }
        
        return score;
    }

    private GridObject GetLowestFcost(List<GridObject> _nodes)
    {
        GridObject lowestFcost = _nodes[0];
        foreach (var node in _nodes)
        {
            if (node.fCost < lowestFcost.fCost)
            {
                lowestFcost = node;
            }
        }
        
        return lowestFcost;
    }
}
