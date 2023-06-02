using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance  { get; private set; }
    
    [Header("Grid Data")]
    [SerializeField] private Transform gridDebugObject;
    [SerializeField] private int dungeonSize, width, height, cellSize;
    [SerializeField] private bool CreateDebugGrid = false;
    [SerializeField] private bool GenerateDungeon = false;
    [SerializeField] private bool GenerateTutorial = false;
    [SerializeField] private List<GridPosition> walkableList;
    [SerializeField] private List<Transform> tutorialLevel;

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

        if (GenerateDungeon)
        {
            GenerateTutorial = false;
        }
        
        if (GenerateDungeon)
        {
            width = dungeonSize;
            height = dungeonSize;
        }
        
        gridSystem = new GridSystem<GridObject>(width, height, cellSize, (GridSystem<GridObject> _grid, GridPosition _gridPosition, Vector3 _worldPosition) => new GridObject(_grid, _gridPosition, _worldPosition));
        SetNeighbours();
        if (CreateDebugGrid)
        {
            gridSystem.CreateDebugObjects(gridDebugObject);
            //gridSystem.CreateWalkableDebugObjects(gridDebugObject, walkableList);
        }
    }

    private void Start()
    {
        DungeonGenerator.Instance.Initialize();
        
        if (GenerateTutorial)
        {
            //walkableList = gridSystem.GetGridPositions();
            walkableList = DungeonGenerator.Instance.GenerateTutorial(tutorialLevel);
        }
        
        if (GenerateDungeon)
        {
            walkableList = DungeonGenerator.Instance.Generate();
        }
        //gridSystem.CreateDebugObjects(DungeonGenerator.Instance.Generate(), gridDebugObject);
        
        if (CreateDebugGrid)
        {
            //gridSystem.CreateDebugObjects(gridDebugObject);
            gridSystem.CreateWalkableDebugObjects(gridDebugObject, walkableList);
        }
    }

    public List<GridPosition> GetWalkableList()
    {
        return walkableList;
    }

    public void SetUnitAtGridObject(GridPosition _gridPosition, Unit _unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(_gridPosition);
        if (!gridObject.HasUnit())
        {
            gridObject.SetUnit(_unit);
        }
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
        Unit occupyingUnit = GetUnitAtGridPosition(_fromGridPosition);
        if (occupyingUnit == _unit)
        {
            ClearUnitAtGridPosition(_fromGridPosition);
        }

        if (!GetGridObject(_toGridPosition).HasUnit())
        {
            SetUnitAtGridObject(_toGridPosition, _unit);
        }
    }

    public bool HasAnyUnit(GridPosition _gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(_gridPosition);
        return gridObject.HasUnit();
    }
    /*public Vector3 GetTargetGridPosition(Vector3 _worldPosition)
    {
        return GetWorldPosition(GetGridPosition(_worldPosition));
    }*/

    /*public List<GridPosition> GetWalkableTilesInCircle(Vector3 _center, float _radius)
    {
        List<GridPosition> inCircleList = gridSystem.GetTilesInCircle(_center, _radius);

        List<GridPosition> validList = new List<GridPosition>();

        foreach (var tile in inCircleList)
        {
            if (walkableList.Contains(tile))
            {
                validList.Add(tile);
            }
        }

        List<GridPosition> result = new List<GridPosition>();
        result = validList;

        return result;
    }*/
    
    public bool insideCircle(Vector3 _center, Vector3 _tile, float _radius) => gridSystem.insideCircle(_center, _tile, _radius);

    public void RemoveUnitAtGridPosition(GridPosition _gridPosition) => gridSystem.RemoveUnitAtGridPosition(_gridPosition);
    public List<GridPosition> GetTilesInCircle(Vector3 _center, float _radius) => gridSystem.GetTilesInCircle(_center, _radius);
    public GridPosition GetGridPosition(Vector3 _worldPosition) => gridSystem.GetGridPosition(_worldPosition);
    public Vector3 GetWorldPosition(GridPosition _gridPosition) => gridSystem.GetWorldPosition(_gridPosition);
    public GridObject GetGridObject(GridPosition _gridPosition) => gridSystem.GetGridObject(_gridPosition);
    
    //public bool IsValidGridPosition(GridPosition _gridPosition) => gridSystem.IsValidGridPosition(_gridPosition);

    public GridObject GetGridObject(Vector3 _worldPosition)
    {
        return gridSystem.GetGridObject(GetGridPosition(_worldPosition));
    }

    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();
    public List<GridPosition> GetGridPositions() => gridSystem.GetGridPositions();
    public void SetNeighbours() => gridSystem.SetNeighbours();
    public List<GridObject> GetNodeGrid() => gridSystem.GetNodeGrid();
}
