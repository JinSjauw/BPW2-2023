using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance  { get; private set; }
    
    [Header("Grid Data")]
    [SerializeField] private Transform gridDebugObject;
    [SerializeField] private int width, height, cellSize;
    [SerializeField] private bool CreateDebugGrid = false;
    [SerializeField] private bool GenerateDungeon = false;
    
    private GridSystem<GridObject> gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one instance of LevelGrid");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        gridSystem = new GridSystem<GridObject>(width, height, cellSize, (GridSystem<GridObject> _grid, GridPosition _gridPosition, Vector3 _worldPosition) => new GridObject(_grid, _gridPosition, _worldPosition));
        SetNeighbours();
        if (CreateDebugGrid)
        {
            gridSystem.CreateDebugObjects(gridDebugObject);
        }
    }

    private void Start()
    {
        if (!GenerateDungeon)
        {
            return;
        }
        DungeonGenerator.Instance.Initialize();
        //gridSystem.CreateDebugObjects(DungeonGenerator.Instance.Generate(), gridDebugObject);
        DungeonGenerator.Instance.Generate();
    }

    public void SetUnitAtGridObject(GridPosition _gridPosition, Unit _unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(_gridPosition);
        gridObject.SetUnit(_unit);
    }

    public void SetUnitAtGridPosition(GridPosition _gridPosition, Unit _unit)
    {
        _unit.transform.position = GetWorldPosition(_gridPosition);
    }
    
    public Unit GetUnitAtGridPosition(GridPosition _gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(_gridPosition);
        
        return gridObject.GetUnit();
    }

    public void ClearUnitAtGridPosition(GridPosition _gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(_gridPosition);
        gridObject.ClearUnit();
    }
    
    public void UnitMovedGridPosition(Unit _unit, GridPosition _fromGridPosition, GridPosition _toGridPosition)
    {
        ClearUnitAtGridPosition(_fromGridPosition);
        
        SetUnitAtGridObject(_toGridPosition, _unit);
    }

    public bool HasAnyUnit(GridPosition _gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(_gridPosition);
        return gridObject.HasUnit();
    }
    public Vector3 GetTargetGridPosition(Vector3 _worldPosition)
    {
        return GetWorldPosition(GetGridPosition(_worldPosition));
    }
    public bool insideCircle(Vector3 _center, Vector3 _tile, float _radius) => gridSystem.insideCircle(_center, _tile, _radius);

    public void RemoveUnitAtGridPosition(GridPosition _gridPosition) => gridSystem.RemoveUnitAtGridPosition(_gridPosition);
    public List<GridPosition> GetTilesInCircle(Vector3 _center, float radius) => gridSystem.GetTilesInCircle(_center, radius);
    public GridPosition GetGridPosition(Vector3 _worldPosition) => gridSystem.GetGridPosition(_worldPosition);
    public Vector3 GetWorldPosition(GridPosition _gridPosition) => gridSystem.GetWorldPosition(_gridPosition);
    public GridObject GetGridObject(GridPosition _gridPosition) => gridSystem.GetGridObject(_gridPosition);
    public bool IsValidGridPosition(GridPosition _gridPosition) => gridSystem.IsValidGridPosition(_gridPosition);
    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();
    public List<GridPosition> GetGridPositions() => gridSystem.GetGridPositions();
    public void SetNeighbours() => gridSystem.SetNeighbours();
    public List<GridObject> GetNodeGrid() => gridSystem.GetNodeGrid();
}
