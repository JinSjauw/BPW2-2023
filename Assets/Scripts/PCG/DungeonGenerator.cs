using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private LevelGrid grid;
    [SerializeField] private List<GridPosition> closedList, openList;
    [SerializeField] private List<Room> roomList;
    [SerializeField] private GameObject tilePrefab;
    
    [Header("Room Data")]
    [SerializeField] private int minRoomWidth;
    [SerializeField] private int minRoomHeight;
    [SerializeField] private int maxRoomWidth;
    [SerializeField] private int maxRoomHeight;

    [SerializeField] private int minAmountRoom;
    [SerializeField] private int maxAmountRoom;

    // Start is called before the first frame update
    void Start()
    {
        grid = LevelGrid.Instance;
        openList = grid.GetGridPositions();

        closedList = new List<GridPosition>();
        roomList = new List<Room>();
    }

    public void SpawnRooms()
    {
        int totalAmount = Random.Range(minAmountRoom, maxAmountRoom);
        int currentAmount = 0;
        bool roomsSpawned = false;
        
        while (!roomsSpawned)
        {
            LoopEnd:
            
            if (openList.Count <= 0)
            {
                Debug.Log("OpenList is empty");
                break;
            }
            
            if (currentAmount < totalAmount)
            {
                //Pick a position
                int positionIndex = Random.Range(0, openList.Count - 1);
                GridPosition gridPosition = openList[positionIndex];
            
                int width = Random.Range(minRoomWidth, maxRoomWidth);
                int height = Random.Range(minRoomHeight, maxRoomHeight);
            
                //Look in the grid.   
                //Verify if the rooms fits on the grid
                if (openList.Contains(new GridPosition(gridPosition.x + width, gridPosition.z + height)))
                {
                    List<GridPosition> roomPositions = new List<GridPosition>();

                    for (int x = 0; x < width; x++)
                    {
                        for (int z = 0; z < height; z++)
                        {
                            GridPosition roomTile = new GridPosition(gridPosition.x + x, gridPosition.z + z);
                            if (openList.Contains(roomTile))
                            {
                                roomPositions.Add(roomTile);
                                openList.Remove(roomTile);
                                closedList.Add(roomTile);
                            }
                            else
                            {
                                Debug.Log("Openlist didn't contain element.");
                                goto LoopEnd;
                            }
                        }
                    }

                    GridPosition center = new GridPosition(gridPosition.x + (width / 2), gridPosition.z + (height / 2));
                    roomList.Add(new Room(currentAmount, width, height, center,ROOMTYPE.NORMAL, roomPositions));
                    
                    currentAmount++;
                }
                
            }
            else
            {
                roomsSpawned = true;
            }
        }

        foreach (var room in roomList)
        {
            List<GridPosition> roomGrid = room.GetRoomGrid();

            foreach (var roomTile in roomGrid)
            {
                Instantiate(tilePrefab, grid.GetWorldPosition(roomTile), Quaternion.identity);
            }
        }
    }
    
    
}
