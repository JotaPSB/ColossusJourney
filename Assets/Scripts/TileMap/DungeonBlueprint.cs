using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonBlueprint : MonoBehaviour
{
    [Range(0,5)]
    public int roomSize;
    [Range(0,5)]
    public int hallSize;
    TileMapTest tileMap;
    // Start is called before the first frame update

    public void RoomWalker()
    {
        tileMap = TileMapTest.instance;
        Vector3Int currentPos = Vector3Int.zero;

        tileMap.AddFloor(currentPos, tileMap.floorSize, tileMap.floorSize);
        while (tileMap.floorList.Count < tileMap.totalFloorCount)
        {
            currentPos = AddHallway(currentPos); AddRoom(currentPos);
           
        }
        StartCoroutine(tileMap.CreateBlueprint());
    }

    Vector3Int AddHallway(Vector3Int myPos)
    {
        Vector3Int walkDirection = tileMap.RandomDirection();
        int walkLength = Random.Range(5 + hallSize, 10 + hallSize);

        for (int i = 0; i < walkLength; i++)
        {
            myPos += walkDirection;
            tileMap.AddFloor(myPos, tileMap.floorSize, tileMap.floorSize);
        }
        return myPos;
    }

    void AddRoom(Vector3Int myPos)
    {

        Vector2Int room = tileMap.RandomRoom(3 + roomSize, 12 + roomSize, 3 + roomSize, 12 + roomSize);
        tileMap.AddFloor(myPos, room.x, room.y);
    }
}
