using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CavernsBlueprint : MonoBehaviour
{
    TileMapTest tileMap;
   

    public void RandomWalker()
    {
        tileMap = TileMapTest.instance;
        Vector3Int currentPos = Vector3Int.zero;
        

        while(tileMap.floorList.Count< tileMap.totalFloorCount)
        {
            tileMap.AddFloor(currentPos, tileMap.floorSize, tileMap.floorSize);
            currentPos += tileMap.RandomDirection();

        }
        StartCoroutine(tileMap.CreateBlueprint());
        
    }
}
