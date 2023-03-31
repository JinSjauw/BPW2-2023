using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GridPosition : IEquatable<GridPosition>
{
   public int x;
   public int z;

   public GridPosition(int _x, int _z)
   {
      x = _x;
      z = _z;
   }
   
   public override string ToString()
   {
      return "x: " + x + " z: " + z;
   }

   public static bool operator ==(GridPosition a, GridPosition b)
   {
      return a.x == b.x && a.z == b.z;
   }
   
   public static bool operator !=(GridPosition a, GridPosition b)
   {
      return !(a == b);
   }

   public bool Equals(GridPosition other)
   {
      return this == other;
   }

   public override bool Equals(object obj)
   {
      return obj is GridPosition position &&
             x == position.x &&
             z == position.z;
   }

   public override int GetHashCode()
   {
      return HashCode.Combine(x, z);
   }
}

public class GridSystem<TGridObject>
{
   private int width;
   private int height;
   private float cellSize;

   private TGridObject[,] gridObjectArray;
   
   public GridSystem(int _width, int _height, float _cellSize, Func<GridSystem<TGridObject>, GridPosition, Vector3, TGridObject> _CreateGridObject)
   {
      width = _width;
      height = _height;
      cellSize = _cellSize;

      gridObjectArray = new TGridObject[width, height];
      for (int x = 0; x < width; x++)
      {
         for (int z = 0; z < height; z++)
         {
            GridPosition gridPosition = new GridPosition(x, z);
            Vector3 worldPosition = GetWorldPosition(gridPosition);
            gridObjectArray[x, z] = _CreateGridObject(this, gridPosition, worldPosition);
         }
      }
   }

   public Vector3 GetWorldPosition(GridPosition _gridPosition)
   {
      return new Vector3(_gridPosition.x, 0, _gridPosition.z) * cellSize;
   }

   public GridPosition GetGridPosition(Vector3 _worldPosition)
   {
      return new GridPosition(
         Mathf.RoundToInt(_worldPosition.x / cellSize),
         Mathf.RoundToInt(_worldPosition.z / cellSize)
      );
   }

   public void CreateDebugObjects(Transform _debugPrefab)
   {
      for (int x = 0; x < width; x++)
      {
         for (int z = 0; z < height; z++)
         {
            GridPosition gridPosition = new GridPosition(x, z);
            Transform debugTransform = GameObject.Instantiate(_debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
            GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
            gridDebugObject.SetGridObject(GetGridObject(gridPosition));
         }
      }
   }

   public void CreateDebugObjects(List<GridPosition> _list, Transform _debugPrefab)
   {
      foreach (var gridPosition in _list)
      {
         Transform debugTransform = GameObject.Instantiate(_debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
         GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
         gridDebugObject.SetGridObject(GetGridObject(gridPosition));
      }
   }

   public GridObject GetGridObject(GridPosition _gridPosition)
   {
      return gridObjectArray[_gridPosition.x, _gridPosition.z] as GridObject;
   }

   public bool IsValidGridPosition(GridPosition _gridPosition)
   {
      return _gridPosition.x >= 0 && 
             _gridPosition.z >= 0 && 
             _gridPosition.x < width && 
             _gridPosition.z < height;
   }

   public int GetWidth()
   {
      return width;
   }

   public int GetHeight()
   {
      return height;
   }

   public void RemoveUnitAtGridPosition(GridPosition _gridPosition)
   {
      GetGridObject(_gridPosition).ClearUnit();
   }
   
   public List<GridPosition> GetGridPositions()
   {
      List<GridPosition> result = new List<GridPosition>();
      for(int x = 0; x < width; x++)
      {
         for (int z = 0; z < height; z++)
         {
            result.Add(new GridPosition(x,z));
         }
      }
        
      return result;
   }
   
   public void SetNeighbours()
    {
        List<GridObject> nodeList = new List<GridObject>();
        List<GridPosition> grid = GetGridPositions();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                //Add neighbours to node
                GridPosition position = new GridPosition(x, z);
                GridObject node = GetGridObject(position);
                
                //Left
                if (grid.Contains(new GridPosition(position.x - 1, position.z)))
                {
                    node.neighbourList.Add(GetGridObject(new GridPosition(position.x - 1, position.z)));
                    
                    //Left Down
                    if (grid.Contains(new GridPosition(position.x - 1, position.z - 1)))
                    {
                        node.neighbourList.Add(GetGridObject(new GridPosition(position.x - 1, position.z - 1)));
                    }
                    if (grid.Contains(new GridPosition(position.x - 1, position.z + 1)))
                    {
                        node.neighbourList.Add(GetGridObject(new GridPosition(position.x - 1, position.z + 1)));
                    }
                }
                
                //Right
                if (grid.Contains(new GridPosition(position.x + 1, position.z)))
                {
                    node.neighbourList.Add(GetGridObject(new GridPosition(position.x + 1, position.z)));
                    
                    //Right Down
                    if (grid.Contains(new GridPosition(position.x + 1, position.z - 1)))
                    {
                        node.neighbourList.Add(GetGridObject(new GridPosition(position.x + 1, position.z - 1)));
                    }
                    //Right Up
                    if (grid.Contains(new GridPosition(position.x + 1, position.z + 1)))
                    {
                        node.neighbourList.Add(GetGridObject(new GridPosition(position.x + 1, position.z + 1)));
                    }
                }

                //Down
                if (grid.Contains(new GridPosition(position.x, position.z - 1)))
                {
                    node.neighbourList.Add(GetGridObject(new GridPosition(position.x, position.z - 1)));
                }
                
                if (grid.Contains(new GridPosition(position.x, position.z + 1)))
                {
                    node.neighbourList.Add(GetGridObject(new GridPosition(position.x, position.z + 1)));
                }
                
                nodeList.Add(node);
            }
        }
    }
   
   public List<GridObject> GetNodeGrid()
   {
      List<GridObject> nodeList = new List<GridObject>();
      for (int x = 0; x < width; x++)
      {
         for (int z = 0; z < height; z++)
         {
            GridObject node = GetGridObject(new GridPosition(x, z));
            nodeList.Add(node);
         }
      }

      return nodeList;
   }
   
   public bool insideCircle(Vector3 _center, Vector3 _tile, float _radius)
   {
      float distanceX = _center.x - _tile.x;
      float distanceZ = _center.z - _tile.z;

      float distance = Mathf.Sqrt(distanceX * distanceX + distanceZ * distanceZ);
      return distance <= _radius;
   }
   
   public List<GridPosition> GetTilesInCircle(Vector3 _center, float radius)
   {
      int top = (int)Mathf.Ceil(_center.z - radius);
      int bottom = (int)Mathf.Floor(_center.z + radius);
      int left = (int)Mathf.Ceil(_center.x - radius);
      int right  = (int)Mathf.Floor(_center.x + radius);

      List<GridPosition> validTiles = new List<GridPosition>();

      for (int z = top; z <= bottom; z++)
      {
         for (int x = left; x <= right; x++)
         {
            GridPosition tile = new GridPosition(x / 2, z / 2);
            if (insideCircle(_center, new Vector3(tile.x * cellSize, 0, tile.z * cellSize), radius))
            {
               if (IsValidGridPosition(tile))
               {
                  validTiles.Add(tile);
               }
            }
         }
      }

      return validTiles;
   }
   
}
