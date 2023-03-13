using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GridObject
{
    private GridSystem<GridObject> gridSystem;
    private GridPosition gridPosition;
    private List<ModuleObject> moduleList; 

    public GridObject(GridSystem<GridObject> _gridSystem, GridPosition _gridPosition)
    {
        gridSystem = _gridSystem;
        gridPosition = _gridPosition;
    }

    public List<ModuleObject> GetModulesList()
    {
        return moduleList;
    }

    public override string ToString()
    {
        return gridPosition.ToString();
    }
}
