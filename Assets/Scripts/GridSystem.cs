using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
   
   public GridSystem(int _width, int _height, float _cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> _CreateGridObject)
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
            gridObjectArray[x, z] = _CreateGridObject(this, gridPosition);
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

   public void CreateDebugObjects(Transform debugPrefab)
   {
      for (int x = 0; x < width; x++)
      {
         for (int z = 0; z < height; z++)
         {
            GridPosition gridPosition = new GridPosition(x, z);
            Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
            GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
            gridDebugObject.SetGridObject(GetGridObject(gridPosition));
         }
      }
   }

   public GridObject GetGridObject(GridPosition _gridPosition)
   {
      return gridObjectArray[_gridPosition.x, _gridPosition.z] as GridObject;
   }
   
}
