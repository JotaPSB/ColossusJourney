using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public static PathFinding instance;
    TileMapTest tileMap;
    List<TileType> openList;
    List<TileType> closedList;
    List<TileType> candList;
    List<TileType> newCands;
    // Start is called before the first frame update
    void Start()
    {
        tileMap = TileMapTest.instance;
        instance = this;
    }
    public List<TileType> FindPath(int startX, int startY, int endX, int endY)
    {
        TileType startTile = tileMap.GetGameTile(new Vector3Int(startX, startY, 0));
        TileType endTile = tileMap.GetGameTile(new Vector3Int(endX, endY, 0));
        openList = new List<TileType> { startTile };
        closedList = new List<TileType>();
        int i = 0; 
        while(i< tileMap.floorList.Count)
        {
            TileType gameTile = tileMap.GetGameTile(new Vector3Int(tileMap.floorList[i].x, tileMap.floorList[i].y,0));
            Debug.Log(i);
            Debug.Log(tileMap.floorList.Count);
            gameTile.gCost = 99999999;
            gameTile.CalculateFCost();
            gameTile.fromTile = null;

            i++;
        }
        startTile.gCost = 0;
        startTile.hCost = CalculateDistanceCost(startTile, endTile);
        startTile.CalculateFCost();

        while(openList.Count > 0)
        {
            TileType currentTile = GetLowerFCost(openList);
            Debug.Log(openList.Count);
            if (currentTile == endTile)
            {
                return CalculatePath(endTile);
            }
            openList.Remove(currentTile);
            closedList.Add(currentTile);
            foreach (TileType neighbourTile in GetNeighbourList(currentTile))
            {
                Debug.Log("Neighbour: " + GetNeighbourList(currentTile).Count);
                if (closedList.Contains(neighbourTile))
                {
                    continue;
                }
                if (!neighbourTile.isWalkable)
                {
                    closedList.Add(neighbourTile);
                    continue;
                }
                int fromCost = currentTile.gCost + CalculateDistanceCost(currentTile, endTile);
                if(fromCost< neighbourTile.gCost)
                {
                    neighbourTile.fromTile = currentTile;
                    neighbourTile.gCost = fromCost;
                    neighbourTile.hCost = CalculateDistanceCost(neighbourTile, endTile);
                    neighbourTile.CalculateFCost();

                    if (!openList.Contains(neighbourTile)) { openList.Add(neighbourTile); }
                }
            }
        }
        return null;
    }


    public void MapDistance(Vector3Int myPos){
        TileType startTile = tileMap.GetGameTile(myPos);

        candList = new List<TileType> { startTile};
        newCands = new List<TileType>();

        for(int i=0; i< tileMap.floorList.Count; i++){
            TileType gameTile = tileMap.GetGameTile(new Vector3Int(tileMap.floorList[i].x, tileMap.floorList[i].y, 0));
            gameTile.gCost = -1;
        }
        startTile.gCost = 0;
        while(candList.Count > 0){
            newCands.Clear();
            foreach(TileType cand in candList){
                foreach(TileType neighbourTile in GetNeighbourList(cand)){
                    if(neighbourTile.isWalkable){
                        if(neighbourTile.gCost == -1){
                            neighbourTile.gCost = cand.gCost + 1;
                            newCands.Add(neighbourTile);
                        }
                    }
                }
            }
            candList.Clear();

            foreach(TileType tile in newCands){
                candList.Add(tile);
            }
        }
    }

    public List<TileType> GetNeighbourList(TileType currentTile){
        List<TileType> neighbourList = new List<TileType>();
        TileType leftTile = tileMap.GetGameTile(new Vector3Int (currentTile.x -1, currentTile.y, 0));
        TileType rightTile = tileMap.GetGameTile(new Vector3Int (currentTile.x +1, currentTile.y, 0));
        TileType downTile = tileMap.GetGameTile(new Vector3Int (currentTile.x, currentTile.y-1, 0));
        TileType upTile = tileMap.GetGameTile(new Vector3Int (currentTile.x, currentTile.y+1, 0));
        
        TileType upLeftTile = tileMap.GetGameTile(new Vector3Int (currentTile.x-1, currentTile.y+1, 0));
        TileType upRightTile = tileMap.GetGameTile(new Vector3Int (currentTile.x+1, currentTile.y+1, 0));
        TileType downLeftTile = tileMap.GetGameTile(new Vector3Int (currentTile.x-1, currentTile.y-1, 0));
        TileType downRightTile = tileMap.GetGameTile(new Vector3Int (currentTile.x+1, currentTile.y-1, 0));
        

        if((leftTile!=null)&&(leftTile.isWalkable)){neighbourList.Add(leftTile);}
        if((rightTile!=null)&&(rightTile.isWalkable)){neighbourList.Add(rightTile);}
        if((downTile!=null)&&(downTile.isWalkable)){neighbourList.Add(downTile);}
        if((upTile!=null)&&(upTile.isWalkable)){neighbourList.Add(upTile);}
        
        if((upLeftTile!=null)&&(upLeftTile.isWalkable)){neighbourList.Add(upLeftTile);}
        if((upRightTile!=null)&&(upRightTile.isWalkable)){neighbourList.Add(upRightTile);}
        if((downLeftTile!=null)&&(downLeftTile.isWalkable)){neighbourList.Add(downLeftTile);}
        if((downRightTile!=null)&&(downRightTile.isWalkable)){neighbourList.Add(downRightTile);}
        Debug.Log("PHNeighList: " + neighbourList.Count);
        return neighbourList;

    }

    private List<TileType> CalculatePath(TileType endTile)
    {
        List<TileType> path = new List<TileType>();

        path.Add(endTile);
        TileType currentTile = endTile;

        while(currentTile.fromTile != null) { path.Add(currentTile.fromTile); currentTile = currentTile.fromTile; }

        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(TileType a, TileType b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        return 1 * remaining;
    }

    private TileType GetLowerFCost(List<TileType> gameTileList)
    {
        TileType lowestFCostTile = gameTileList[0];

        for(int i=0; i < gameTileList.Count; i++)
        {
            if (gameTileList[i].fCost < lowestFCostTile.fCost)
            {
                lowestFCostTile = gameTileList[i];
            }
        }
        return lowestFCostTile;
    }


}
