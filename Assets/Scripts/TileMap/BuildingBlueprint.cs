using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingBlueprint : MonoBehaviour
{
    public int maxRoomWidth;
    public int maxRoomHeight;

    int areaNum = 1;
    int areaFound;

    List<Vector3Int> roomFloor = new List<Vector3Int>();
    List<Vector2Int> areaList = new List<Vector2Int>();

    TileMapTest tileMap;

    public void FloorPlan(){
        tileMap = TileMapTest.instance;
        
        AddRooms();
        AddCorridors();
        
    }

    void AddRooms(){
        int maxRooms = 20; int maxFails= 10;
        while(maxRooms>0 && maxFails> 0){
            Vector2Int room = tileMap.RandomRoom(5, maxRoomWidth, 5, maxRoomHeight);
            
            
            if(PlaceRoom(room)){
                maxRooms -= 1;
                
            }else{
                maxFails -= 1;
                if(room.x > room.y){
                    if(maxRoomWidth>5){
                        maxRoomWidth-=1;
                    }
                }else{
                    if(maxRoomHeight>5){
                        maxRoomHeight-=1;
                    }
                }
            }
        }
    }
    bool PlaceRoom(Vector2Int room){
        List<Vector2Int> candList = new List<Vector2Int>();

        for(int w = (-tileMap.mapSize.x/2); w<= ((tileMap.mapSize.x /2)-room.x); w++){
            for(int h=(-tileMap.mapSize.y/2); h<= ((tileMap.mapSize.y /2)-room.y); h++){
                if(RoomFits(room, w, h)){
                    candList.Add(new Vector2Int(w, h));
                }
            }
        }

        if(candList.Count == 0){
            return false;
        }else{
            Vector2Int selectCand = candList[Random.Range(0, candList.Count-1)];
            for(int w = 0; w < room.x; w++){
                for (int h=0; h <room.y; h++){
                    
                    Vector3Int tilePos = new Vector3Int(selectCand.x +w, selectCand.y +h, 1);
                    
                    tileMap.AddTile(tilePos.x, tilePos.y,tileMap.tileTypes[Random.Range(0,3)], areaNum);
                    tileMap.AddWalls(tilePos);
                    roomFloor.Add(tilePos);
                }
            }
            areaNum++;
        }
        return true;
    }

    bool RoomFits(Vector2Int room, int x, int y){
        for(int w =-1; w<= room.x; w++){
            for(int h=-1; h<= room.y; h++){
                Vector3Int tilePos = new Vector3Int(x+w, y+h, 0);
                TileType tile;
                if(tileMap.tiles.TryGetValue(tilePos, out tile)){
                    if(tile.isWalkable){return false;}
                }
            }
        }
        return true;
    }

    void AddCorridors(){
        for(int w = (-tileMap.mapSize.x/2); w<= ((tileMap.mapSize.x /2)); w++){
            for(int h=(-tileMap.mapSize.y/2); h<= ((tileMap.mapSize.y /2)); h++){
                Vector3Int tilePos = new Vector3Int(w, h, 0);
                if(tileMap.GetSignature(w,h) == 0B111111111111111111111111){
                  tileMap.AddFloor(tilePos, 1, 1);
                }
            }
        }
        StartCoroutine(tileMap.CreateBlueprint());
    }

    public void UpdateFloor(){
        int i=0;
        while(i< roomFloor.Count){
            Vector3Int floorPos = new Vector3Int(roomFloor[i].x, roomFloor[i].y, 0);
            tileMap.floorList.Add(floorPos);
            i++;
        }

        CarveDoors();
    }

    void CarveDoors(){
        int num1 = 0;
        int num2 = 0;
        bool foundDoor = false;
        Dictionary<Vector3Int, Vector2Int> doorCands = new Dictionary<Vector3Int, Vector2Int>();

        foreach(Vector3Int pos in tileMap.allTilesVectors){
            TileType tile, tile2, tile3;
            if(tileMap.tiles.TryGetValue(pos, out tile)){
                if(!tile.isWalkable){
                    int sig = tileMap.GetSignature(pos.x, pos.y);
                    if(tileMap.BinaryComparison(sig, 0B000000000011110000000000,0B100011000110011000110001)){
                        if(tileMap.tiles.TryGetValue(new Vector3Int(pos.x -1, pos.y, pos.z),out tile2)){
                            num1 = tile2.areaNumber;
                        }
                        if(tileMap.tiles.TryGetValue(new Vector3Int(pos.x +1, pos.y, pos.z),out tile3)){
                            num2 = tile3.areaNumber;
                        }
                        if(!InAreaList(new Vector2Int(num1, num2))){
                            areaList.Add( new Vector2Int(num1, num2));
                        }
                        foundDoor = true;
                    }
                    else if(tileMap.BinaryComparison(sig, 0B001000010000000010000100,0B111110000000000000011111)){
                        if(tileMap.tiles.TryGetValue(new Vector3Int(pos.x, pos.y -1, pos.z),out tile2)){
                            num1 = tile2.areaNumber;
                        }
                        if(tileMap.tiles.TryGetValue(new Vector3Int(pos.x, pos.y +1, pos.z),out tile3)){
                            num2 = tile3.areaNumber;
                        }
                        if(!InAreaList(new Vector2Int(num1, num2))){
                            areaList.Add( new Vector2Int(num1, num2));
                        }
                        foundDoor = true;
                    }
                    if(foundDoor && (num1 != num2)){
                        doorCands.Add(pos, new Vector2Int(num1, num2));
                    }
                }
            }
            foundDoor = false;
        }
        System.Random rand = new System.Random();
        doorCands = doorCands.OrderBy(x => rand.Next()).ToDictionary(item => item.Key, ContextMenuItemAttribute => ContextMenuItemAttribute.Value);

        foreach (KeyValuePair<Vector3Int, Vector2Int> doorCand in doorCands){
            if(InAreaList(doorCand.Value)){
                tileMap.AddTile(doorCand.Key.x, doorCand.Key.y, tileMap.tileTypes[Random.Range(0, 3)], areaList[areaFound].x);
                tileMap.floorList.Add(doorCand.Key);
                areaList.RemoveAt(areaFound);
            }
        }
    }

    bool InAreaList(Vector2Int areaNum){
        for(int i=0; i < areaList.Count; i++){
            if(Vector2Int.Equals(areaNum, areaList[i])){
                areaFound = i;
                return true;
            }
        }
        return false;
    }
}
