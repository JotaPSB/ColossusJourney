using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileMap : MonoBehaviour
{
    public GameObject selectedUnit;
    public TileType[] tileTypes;
    public int seed;
    public bool randomSeed;
    int[,] tiles;
    Node[,] graph;
    int mapSizeX = 40;
    int mapSizeY = 40;
    [Range(1, 4)]
    public int floorSize;
    public int totalFloorCount;

    CavernsBlueprint cavernsBlueprint;


    private void Start() {
        selectedUnit.GetComponent<Unit>().tileX = (int) selectedUnit.transform.position.x;
        selectedUnit.GetComponent<Unit>().tileY = (int) selectedUnit.transform.position.y;
        //selectedUnit.GetComponent<Unit>().map = this;
        cavernsBlueprint = GetComponent<CavernsBlueprint>();
        GenerateMapData();
        GeneratePathfindingGraph();
        GenerateMapVisual();
    }



    void GenerateMapData(){
        tiles = new int[mapSizeX, 
        mapSizeY];


        
        for(int x = 0; x<mapSizeX; x++){
            for(int y = 0; y<mapSizeX; y++){
                tiles[x,y] = Random.Range(0,3);
            }
        }

        tiles[4,4] = 3;
        tiles[5,4] = 3;
        tiles[6,4] = 3;
        tiles[7,4] = 3;
        tiles[8,4] = 3;

        tiles[4, 5] = 3;
        tiles[4, 6] = 3;
        tiles[8, 5] = 3;
        tiles[8, 6] = 3;

    }

   

    void GeneratePathfindingGraph(){
        graph= new Node[mapSizeX, mapSizeY];
        for(int x = 0; x<mapSizeX; x++){
            for(int y = 0; y<mapSizeY; y++){
                graph[x, y] = new Node(1,1);
                graph[x, y].x = x;
                graph[x,y].y = y;
            }
        }
        for(int x = 0; x<mapSizeX; x++){
            for(int y = 0; y<mapSizeY; y++){
            
 /*
                if(x>0)
                    graph[x,y].neighbours.Add(graph[x-1, y]);
                if(x<mapSizeX-1)
                    graph[x,y].neighbours.Add(graph[x+1, y]);
                if(y>0)
                    graph[x,y].neighbours(int) transform.position.x +.Add(graph[x, y-1]);
                if(y<mapSizeY-1)
                    graph[x,y].neighbours.Add(graph[x, y+1]);
                    */

                if(x>0){
                    graph[x,y].neighbours.Add(graph[x-1, y]);
                    if(y>0)
                        graph[x,y].neighbours.Add(graph[x-1, y-1]);
                    if(y<mapSizeY-1)
                        graph[x,y].neighbours.Add(graph[x-1, y+1]);
                }
                if(x<mapSizeX-1){
                    graph[x,y].neighbours.Add(graph[x+1, y]);
                    if(y>0)
                        graph[x,y].neighbours.Add(graph[x+1, y-1]);
                    if(y<mapSizeY-1)
                        graph[x,y].neighbours.Add(graph[x+1, y+1]);
                }
                    
                if(y>0)
                    graph[x,y].neighbours.Add(graph[x, y-1]);
                if(y<mapSizeY-1)
                    graph[x,y].neighbours.Add(graph[x, y+1]);
            }

        }
    }

    void GenerateMapVisual(){
        for(int x = 0; x<mapSizeX; x++){
            for(int y = 0; y<mapSizeY; y++){
                TileType tt = tileTypes[tiles[x,y]];
                GameObject go = (GameObject) Instantiate(tt.tileVisual, new Vector3(x,y, 0), Quaternion.identity);
                ClickableTileTest ct = go.GetComponent<ClickableTileTest>();
                ct.tileX = x;
                ct.tileY = y;
                //ct.map = this;

            }
        }
    }
    public Vector3 TileCoordToWorldCoord(int x, int y)
    {
        return new Vector3(x, y, 0);
    }
    float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY){
        TileType tt = tileTypes[tiles[targetX,targetY]];
        if(UnitCanEnterTile(targetX, targetY)== false){
            return Mathf.Infinity;
        }
        float cost = tt.movementCost;
        if(sourceX!=targetX && sourceY!=targetY){
            cost+= 0.001f;
        }

        return cost;
    }

    public bool UnitCanEnterTile(int x, int y){
        return tileTypes[tiles[x,y]].isWalkable;
    }
    public void GeneratePathTo(int x, int y)
    {
        selectedUnit.GetComponent<Unit>().currentPath = null;

        if(UnitCanEnterTile(x,y)== false){
            return;
        }
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        List<Node> unvisited = new List<Node>();

        Node source = graph[
            selectedUnit.GetComponent<Unit>().tileX, 
            selectedUnit.GetComponent<Unit>().tileY];
        Node target = graph[
            x, 
            y];

        dist[source] = 0;
        prev[source] = null;

        foreach(Node v in graph){
            if(v!= source){
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }

            unvisited.Add(v);
        }

        while(unvisited.Count>0){
            Node u = null;

            foreach(Node possibleU in unvisited){
                if(u ==null || dist[possibleU] < dist[u]){
                    u = possibleU;
                }
            }

            if(u == target){
                break;
            }

            unvisited.Remove(u);
            foreach(Node v in u.neighbours){
                //float alt = dist[u] + u.DistanceTo(v);
                float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x,v.y);
                if(alt< dist[v]){
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        if(prev[target] == null){

            return;
        }

        List<Node> currentPath = new List<Node>();
        Node curr = target;

        while(curr != null){
            currentPath.Add(curr);
            curr = prev[curr];
        }

        currentPath.Reverse();

        selectedUnit.GetComponent<Unit>().currentPath = currentPath;
    }
    
}
