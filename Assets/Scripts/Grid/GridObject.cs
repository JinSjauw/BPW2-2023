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
    
    private List<Unit> unitList;


    public GridObject(GridSystem<GridObject> _gridSystem, GridPosition _gridPosition, Vector3 _position)
    {
        gridSystem = _gridSystem;
        gridPosition = _gridPosition;
        position = _position;
        tileType = TILETYPE.EMPTY;
        neighbourList = new List<GridObject>();
        unitList = new List<Unit>();
    }
    
    public void SetUnit(Unit _unit)
    {
        unitList.Add(_unit);
    }

    public Unit GetUnit()
    {
        if (HasUnit())
        {
            return unitList[0];
        }
        else
        {
            return null;
        }
    }

    public void ClearUnit()
    {
        unitList.Clear();
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
        return unitList.Count > 0;
    }

    public override string ToString()
    {
        Unit currentUnit = null;
        if (unitList.Count > 0)
        {
            currentUnit = unitList[0];
        }
        
        return gridPosition.ToString() + "\r\n" +  currentUnit + " " + tileType;
    }
}
