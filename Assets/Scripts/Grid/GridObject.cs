using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public enum TILETYPE
{
    EMPTY = -1,
    FLOOR = 0,
    WALL = 1,
    DOORWAY = 2,
}

public class GridObject
{
    private GridSystem<GridObject> gridSystem;
    public GridPosition gridPosition { get; private set; }
    public TILETYPE tileType { get; private set; }

    public GridObject(GridSystem<GridObject> _gridSystem, GridPosition _gridPosition)
    {
        gridSystem = _gridSystem;
        gridPosition = _gridPosition;
    }

    public override string ToString()
    {
        return gridPosition.ToString();
    }
}
