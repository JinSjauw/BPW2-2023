using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Node
{
    public Vector3 position;
    
    public int fCost;
    public int gCost;
    public int hCost;

    public Node parent = null;
    public List<Node> neighbourList;
        
    public Node(Vector3 _position)
    {
        position = _position;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
}

public class Pathfinding
{
    //CONST
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private const int MOVE_THROUGH = 5;
    
    private LevelGrid grid;
    private List<Node> nodeGrid;
    private List<Node> openList;
    private List<Node> closedList;

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

    public List<Node> FindPath(Node _start, Node _end)
    {
        Node startNode = _start;
        Node endNode = _end;

        openList = new List<Node> { startNode };
        closedList = new List<Node>();

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
            Node currentNode = GetLowestFcost(openList);

            if (currentNode == endNode)
            {
                //Return path here. End has been reached
                return null;
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

            return null;
        }
        
        
        return null;
    }

    public List<Node> ReturnPath()
    {
        //Reverse List
        return null;
    }

    private int CalculateDistance(Node _a, Node _b)
    {
        if(_a == null || _b == null)
        {
            Debug.Log("Pathnode missing");
            return 0;
        }

        int distanceX = (int)MathF.Abs(_a.position.x - _b.position.z);
        int distanceY = (int)MathF.Abs(_a.position.x - _b.position.z);
        int remaining = Mathf.Abs(distanceX - distanceY);

        return MOVE_DIAGONAL_COST * Mathf.Min(distanceX, distanceY) + MOVE_STRAIGHT_COST * remaining;
    }

    private Node GetLowestFcost(List<Node> _nodes)
    {
        Node lowestFcost = _nodes[0];
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
