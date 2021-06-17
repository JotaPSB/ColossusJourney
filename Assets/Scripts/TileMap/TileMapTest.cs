using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapTest : MonoBehaviour
{
    
    public GameObject selectedUnit;
    public TileType[] tileTypes;
    public int seed;
    public bool randomSeed;
    
    public Vector2Int mapSize;
    Dictionary<Vector3,Node> graph;
    int maxXValue;
    int maxYValue;
    int minXValue;
    int minYValue;
    [Range(0, 3)]
    public int floorSize;
    public int totalFloorCount;

    public static TileMapTest instance;

    public enum BlueprintType { Caverns, Dungeon, Building}
    public BlueprintType blueprintType;

    //int[,] tiles;
    public Dictionary<Vector3, TileType> tiles;
    public List<Vector3Int> floorList = new List<Vector3Int>();

    public List<Vector3Int> allTilesVectors = new List<Vector3Int>();
    CavernsBlueprint cavernsBlueprint;
    DungeonBlueprint dungeonBlueprint;
    BuildingBlueprint buildingBlueprint; 

    private void Awake()
    {
        instance = this;
        tiles = new Dictionary<Vector3, TileType>();
        selectedUnit.GetComponent<Unit>().tileX = (int)selectedUnit.transform.position.x;
        selectedUnit.GetComponent<Unit>().tileY = (int)selectedUnit.transform.position.y;
        selectedUnit.GetComponent<Unit>().map = this;
        if (randomSeed)
        {
            seed = Random.Range(0, 99999);
        }
        Random.InitState(seed);
        cavernsBlueprint = GetComponent<CavernsBlueprint>();
        dungeonBlueprint = GetComponent<DungeonBlueprint>();
        buildingBlueprint = GetComponent<BuildingBlueprint>();

        switch (blueprintType)
        {
            case BlueprintType.Caverns: cavernsBlueprint.RandomWalker(); break;
            case BlueprintType.Dungeon: dungeonBlueprint.RoomWalker(); break;
            case BlueprintType.Building: buildingBlueprint.FloorPlan(); break;
        }
        

    }



    public void AddTile(int x, int y,TileType tileType,int areaNum)
    {
        /*
        tiles = new int[mapSizeX,
        mapSizeY];



        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeX; y++)
            {
                tiles[x, y] = Random.Range(0, 3);
            }
        }

        tiles[4, 4] = 3;
        tiles[5, 4] = 3;
        tiles[6, 4] = 3;
        tiles[7, 4] = 3;
        tiles[8, 4] = 3;

        tiles[4, 5] = 3;
        tiles[4, 6] = 3;
        tiles[8, 5] = 3;
        tiles[8, 6] = 3;
        */
        
        if (x > maxXValue)
        {
            maxXValue = x;
        }
        if (x < minXValue)
        {
            minXValue = x;
        }
        if (y> maxYValue)
        {
            maxYValue = y;
        }
        if (y< minYValue)
        {
            minYValue = y;
        }

         
        TileType tt;
        if(tiles.TryGetValue(new Vector3(x,y,0),out tt)){
            tiles.Remove(new Vector3(x,y,0));
        
            
        }
        tileType.areaNumber= areaNum;
        tileType.x =x;
        tileType.y = y;
        tiles.Add(new Vector3(x, y, 0), tileType);
 
        



    }

    public void AddFloor(Vector3Int currentPos, int width, int height){
         for(int x= -width; x<= width; x++)
            {
                for(int y= -height; y<= height; y++)
                {
                    Vector3Int offset = new Vector3Int(x, y, 0);
                    if(!InFloorList(currentPos + offset))
                    {
                        floorList.Add(currentPos + offset);
                    }
                }
            }
    }
    public void AddWalls(Vector3Int currentPos){
        for(int x= -1; x<= 1; x++)
            {
                for(int y= -1; y<= 1; y++)
                {
                    Vector3Int wallPos = new Vector3Int(currentPos.x + x, currentPos.y + y, 0);
                    if (tiles.ContainsKey(wallPos)) continue;
                    AddTile(wallPos.x,wallPos.y, tileTypes[8],-1);
                }
            }
    }
    public void ChangeWalkTile(Vector3 key, bool walkable)
    {
        TileType tt;
        if (tiles.TryGetValue(key, out tt))
        {
            if (tt.name != "WallDungeon")
            {
                tt.isWalkable = walkable;
                tiles[key] = tt;
            }
        }
    }
    public Vector3Int RandomDirection()
    {
        switch (Random.Range(1, 5))
        {
            case 1: return new Vector3Int(0, 1 + (floorSize * 2), 0);
            case 2: return new Vector3Int(1 + (floorSize * 2), 0, 0);
            case 3: return new Vector3Int(0, -1 - (floorSize * 2), 0);
            case 4: return new Vector3Int(-1 - (floorSize * 2), 0, 0);
        }
        return Vector3Int.zero;
    }
    public Vector2Int RandomRoom(int minRoomWidth, int maxRoomWidth, int minRoomHeight, int maxRoomHeight){
        int width = Random.Range(minRoomWidth, maxRoomWidth);
        int height = Random.Range(minRoomHeight, maxRoomHeight);

        return new Vector2Int(width, height);
    }

    //00000
    //00000
    //11*00
    //11111
    //11111

    public int GetSignature(int xPos, int yPos){
        int i = 1;
        int signiture = 0;
        int digit = 0;

        for(int x= -2; x<= 2; x++){
            for(int y = -2; y<=2; y++){
                if(x != 0 || y != 0){
                    Vector3Int offsetPos = new Vector3Int(xPos + x, yPos + y, 0);
                    TileType tile;
                    if(tiles.TryGetValue(offsetPos, out tile)){
                        if(tile.isWalkable){
                            digit = 0;
                        }else{
                            digit = 1;
                        }
                    }else{
                        digit =1;
                    }
                    signiture = signiture | digit << 24 -i;

                    i++;
                }
            }
        }
        return signiture;
    }

    public bool BinaryComparison(int sig, int match, int mask){
        return (sig | mask) == (match | mask);
    }

    public bool InFloorList(Vector3Int myPos)
    {
        for(int i = 0; i <floorList.Count; i++)
        {
            if(Vector3Int.Equals(myPos, floorList[i])) { return true; }
        }
        return false;
    }

    public IEnumerator CreateBlueprint()
    {
        

        int i = 0;
        while(i< floorList.Count)
        {
            var localPos = new Vector3Int(floorList[i].x, floorList[i].y, floorList[i].z);
            AddTile(localPos.x,localPos.y,tileTypes[Random.Range(0,8)], 0);
            AddWalls(localPos);

            i++;
            yield return new WaitForSeconds(0f);
        }
        if(blueprintType == BlueprintType.Building){
            buildingBlueprint.UpdateFloor();
        }
        GameManager.instance.Init();  
        GenerateMapVisual();
        GeneratePathfindingGraph();
              
  
    }

    public TileType GetGameTile(Vector3 myPos){
        TileType tile;
        if(tiles.TryGetValue(myPos, out tile)){
            return tile;
        }
        return null;
    }


    public void GeneratePathfindingGraph()
    {


        graph = new Dictionary<Vector3, Node>();
        foreach (KeyValuePair<Vector3, TileType> tile in tiles) {
            graph.Add(tile.Key, new Node((int)tile.Key.x, (int)tile.Key.y));
        }

        for (int x = minXValue; x <= maxXValue; x++)
        {
            for (int y = minYValue; y <= maxYValue; y++)
            {





                Node n;
                if (graph.TryGetValue(new Vector3(x, y, 0), out n))
                {
                    //Debug.Log(n.x + " " + n.y);
                    Node n1;
                    if (x > minXValue)
                    {

                        if (graph.TryGetValue(new Vector3(x - 1, y, 0), out n1)) n.neighbours.Add(n1);
                        if (y > minYValue)
                            if (graph.TryGetValue(new Vector3(x - 1, y - 1, 0), out n1)) n.neighbours.Add(n1);
                        if (y < maxYValue - 1)
                            if (graph.TryGetValue(new Vector3(x - 1, y + 1, 0), out n1)) n.neighbours.Add(n1);
                    }
                    if (x < maxXValue - 1)
                    {
                        if (graph.TryGetValue(new Vector3(x + 1, y, 0), out n1)) n.neighbours.Add(n1);
                        if (y > minYValue)
                            if (graph.TryGetValue(new Vector3(x + 1, y - 1, 0), out n1)) n.neighbours.Add(n1);
                        if (y < maxYValue - 1)
                            if (graph.TryGetValue(new Vector3(x + 1, y + 1, 0), out n1)) n.neighbours.Add(n1);
                    }

                    if (y > minYValue)
                        if (graph.TryGetValue(new Vector3(x, y - 1, 0), out n1)) n.neighbours.Add(n1);
                    if (y < maxYValue - 1)
                        if (graph.TryGetValue(new Vector3(x, y + 1, 0), out n1)) n.neighbours.Add(n1);
                }

            }
        }
    }

    void GenerateMapVisual()
            {
    foreach (KeyValuePair<Vector3, TileType> tile in tiles) {
        GameObject go = (GameObject)Instantiate(tile.Value.tileVisual, tile.Key, Quaternion.identity);
        ClickableTileTest ct = go.GetComponent<ClickableTileTest>();
        ct.tileX = (int)tile.Key.x;
        ct.tileY = (int)tile.Key.y;
        ct.map = this;
        }
    }
    public Vector3 TileCoordToWorldCoord(int x, int y)
    {
        return new Vector3(x, y, 0);
    }
    public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY)
    {
        TileType tt;
        if (tiles.TryGetValue(new Vector3(targetX, targetY), out tt))
        {
            if (UnitCanEnterTile(targetX, targetY) == false )
            {
                return Mathf.Infinity;
            }
            if(IsEnemyOnTile(new Vector3(targetX, targetY, 0)))
            {
                return Mathf.Infinity;
            }
            float cost = tt.movementCost;
            if (sourceX != targetX && sourceY != targetY)
            {
                cost += 0.001f;
            }

            return cost;


        }
        return Mathf.Infinity;
    }

    public bool UnitCanEnterTile(int x, int y)
    {
        TileType tt;
        if (tiles.TryGetValue(new Vector3(x, y, 0), out tt))
        {
            return tt.isWalkable;
        }
        return false;
    }
    
    public void PlayerGeneratePathTo(int x, int y)
    {
        selectedUnit.GetComponent<Unit>().currentPath = null;

        if (UnitCanEnterTile(x, y) == false)
        {
            return;
        }
        if(IsEnemyOnTile(new Vector3(x, y, 0))){
            return;
        }
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        List<Node> unvisited = new List<Node>();

        Node source;
        if (graph.TryGetValue(new Vector3(selectedUnit.GetComponent<Unit>().tileX, selectedUnit.GetComponent<Unit>().tileY), out source))
        {
            //Debug.Log(source.x + " " + source.y);


            Node target;
            if (graph.TryGetValue(new Vector3(x, y), out target))
            {


                dist[source] = 0;
                prev[source] = null;


                foreach (KeyValuePair<Vector3, Node> v in graph)
                {
                    //Debug.Log(v.Key);
                    if (v.Value != source)
                    {
                        dist[v.Value] = Mathf.Infinity;
                        prev[v.Value] = null;
                    }

                    unvisited.Add(v.Value);
                }

                while (unvisited.Count > 0)
                {
                    Node u = null;

                    foreach (Node possibleU in unvisited)
                    {
                        if (u == null || dist[possibleU] < dist[u])
                        {
                            u = possibleU;
                        }
                    }

                    if (u == target)
                    {
                        break;
                    }

                    unvisited.Remove(u);
                    foreach (Node v in u.neighbours)
                    {
                        //float alt = dist[u] + u.DistanceTo(v);
                        float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y);
                        if (alt < dist[v])
                        {
                            dist[v] = alt;
                            prev[v] = u;
                        }
                    }
                }

                if (prev[target] == null)
                {

                    return;
                }

                List<Node> currentPath = new List<Node>();
                Node curr = target;

                while (curr != null)
                {

                    currentPath.Add(curr);
                    curr = prev[curr];
                }

                currentPath.Reverse();

                selectedUnit.GetComponent<Unit>().currentPath = currentPath;
            }
        }
    }
    
    public List<Node> GeneratePathTo(int x, int y, Transform transform)
    {
        List<Node> currentPath = null;

        if (UnitCanEnterTile(x, y) == false)
        {
            return null;
        }
        if (IsEnemyOnTile(new Vector3(x, y, 0)))
        {
            return null;
        }
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        List<Node> unvisited = new List<Node>();

        Node source;
        if (graph.TryGetValue(new Vector3(transform.position.x, transform.position.y), out source))
        {
            //Debug.Log(source.x + " " + source.y);


            Node target;
            if (graph.TryGetValue(new Vector3(x, y), out target))
            {


                dist[source] = 0;
                prev[source] = null;


                foreach (KeyValuePair<Vector3, Node> v in graph)
                {
                    //Debug.Log(v.Key);
                    if (v.Value != source)
                    {
                        dist[v.Value] = Mathf.Infinity;
                        prev[v.Value] = null;
                    }

                    unvisited.Add(v.Value);
                }

                while (unvisited.Count > 0)
                {
                    Node u = null;

                    foreach (Node possibleU in unvisited)
                    {
                        if (u == null || dist[possibleU] < dist[u])
                        {
                            u = possibleU;
                        }
                    }

                    if (u == target)
                    {
                        break;
                    }

                    unvisited.Remove(u);
                    foreach (Node v in u.neighbours)
                    {
                        //float alt = dist[u] + u.DistanceTo(v);
                        float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y);
                        if (alt < dist[v])
                        {
                            dist[v] = alt;
                            prev[v] = u;
                        }
                    }
                }

                if (prev[target] == null)
                {

                    return null;
                }

                currentPath = new List<Node>();
                Node curr = target;

                while (curr != null)
                {

                    currentPath.Add(curr);
                    curr = prev[curr];
                }

                currentPath.Reverse();

               
            }
        }
        return currentPath;
    }

    private bool IsEnemyOnTile(Vector3 pos)
    {
        Collider2D[] colliders;
        colliders = Physics2D.OverlapCircleAll(pos, 0.1f /* Radius */);
        if (colliders.Length > 1) 
        {

            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.tag == "Floor")
                { 
                    return collider.gameObject.GetComponent<ClickableTileTest>().isSomethingOnTile;
                }
            }
        }
        return false;
    }

}
