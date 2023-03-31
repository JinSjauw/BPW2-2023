using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public enum TILETYPE
{
    EMPTY = -1,
    FLOOR = 0,
    WALL = 1,
    DOORWAY = 2,
    HALLWAY = 3,
}

public class GridObject
{
    private GridSystem<GridObject> gridSystem;
        //public Node gridNode;
    public GridPosition gridPosition { get; private set; }
    public TILETYPE tileType { get; private set; }

    //Pathfinding
    public Vector3 position { get; private set; }
    public int fCost;
    public int gCost;
    public int hCost;

    public GridObject parent = null;
    public List<GridObject> neighbourList;
    
    private Unit currentUnit;


    public GridObject(GridSystem<GridObject> _gridSystem, GridPosition _gridPosition, Vector3 _position)
    {
        gridSystem = _gridSystem;
        gridPosition = _gridPosition;
        position = _position;
        tileType = TILETYPE.EMPTY;
        neighbourList = new List<GridObject>();
    }
    
    public void SetUnit(Unit _unit)
    {
        currentUnit = _unit;
    }

    public Unit GetUnit()
    {
        return currentUnit;
    }
    
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetTileType(TILETYPE _type)
    {
        tileType = _type;
    }

    public bool HasUnit()
    {
        return currentUnit != null;
    }
    
    public override string ToString()
    {
        return gridPosition.ToString() + "\r\n" +  currentUnit;
    }
}
