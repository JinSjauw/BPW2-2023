using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ROOMTYPE
{
   NORMAL = 0,
   START = 1,
   END = 2,
   TREASURE = 3,
}

[System.Serializable]
public class Room
{
    public int id;
    public int width;
    public int height;
    public GridPosition roomCenter;
    public ROOMTYPE roomType;
   
   [SerializeField] private List<GridPosition> roomGrid;

   public Room(int _id, int _width, int _height, GridPosition _roomCenter, ROOMTYPE _roomType, List<GridPosition> _roomGrid)
   {
       id = _id;
       width = _width;
       height = _height;
       roomCenter = _roomCenter;
       roomType = _roomType;
       roomGrid = _roomGrid;
   }

   public Room()
   {
   }

   public void SetSize(int _width, int _height)
   {
       width = _width;
       height = _height;
   }

   public List<GridPosition> GetRoomGrid()
   {
       return roomGrid;
   }
}
