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
    
    private List<GridObject> nodeGrid;
    private List<GridObject> openList;
    private List<GridObject> closedList;

    public Pathfinding()
    {
        if (LevelGrid.Instance == null)
        {
            Debug.Log("ERROR: No level grid");
        }
    }
    
    //Breadth First Search

    public List<GridPosition> FindPossiblePaths(List<GridPosition> _nodeGrid, GridPosition _origin, float _maxDistance)
    {
        Dictionary<GridObject, int> visited = new Dictionary<GridObject, int>();
        Queue<GridObject> unvisited = new Queue<GridObject>();

        GridObject origin = LevelGrid.Instance.GetGridObject(_origin);

        visited[origin] = 0;
        unvisited.Enqueue(origin);
        int distance = 0;
        
        while (unvisited.Count > 0 && distance < _maxDistance)
        {
            GridObject current = unvisited.Dequeue();
            foreach (var neighbour in current.neighbourList)
            {
                if (!visited.ContainsKey(neighbour) && _nodeGrid.Contains(neighbour.gridPosition))
                {
                    unvisited.Enqueue(neighbour);
                    visited[neighbour] = 1 + visited[current];
                }
            }
            distance = visited[current];
        }

        List<GridPosition> validTiles = new List<GridPosition>();

        foreach (var tile in visited)
        {
            validTiles.Add(tile.Key.gridPosition);
        }

        return validTiles;
    }

    //A* Pathfinding
    
    public void SetGrid(List<GridPosition> _nodeGrid)
    {
        List<GridObject> tempList = new List<GridObject>();
        foreach (var node in _nodeGrid)
        {
            GridObject gridObj = LevelGrid.Instance.GetGridObject(node);
            tempList.Add(gridObj);
        }

        nodeGrid = tempList;
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

            if (currentNode.gridPosition == endNode.gridPosition)
            {
                //Return path here. End has been reached
                return ReturnPath(startNode, endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbour in currentNode.neighbourList)
            {
                if (closedList.Contains(neighbour) || !nodeGrid.Contains(neighbour))
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

        if (_a.tileType != TILETYPE.EMPTY)
        {
            score -= 2;
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
