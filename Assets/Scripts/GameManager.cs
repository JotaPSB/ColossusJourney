using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    TileMapTest tileMap;

    public PathFinding pathFinding;
    public bool playerTurn = true;
    public List<GameObject> enemiesAvailable;
    public int enemiesAvailableInThisFloor=2;
    public List<GameObject> obstacleAvailable;
    public GameObject treasureChest;
    public Throwable objectPlayerIsThrowing;

    public List<GameObject> enemiesSpawned;
    private List<Vector3Int> positionsUsed;
    public int howManyEnemies;
    public GameObject adviceText;
    public bool isPlayerThrowing;

    public float timeBtwActions;
    public float startTimeAction;
    public int actualFloor;
    public int maxNumFloor;
    public Text floorText;
    public bool isDead = false;
   


    public GameObject player, enemy;
    public GameObject[] obstacles;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        
    }

    private void Start() {
        tileMap = TileMapTest.instance;
        howManyEnemies = 7;
        startTimeAction = 0.20f;
        timeBtwActions = 0;
        maxNumFloor = 5;
        isPlayerThrowing = false;
        actualFloor = PlayerPrefs.GetInt("actualFloor");
        if (actualFloor == 0)
        {
            actualFloor = 1;
        }
        
        positionsUsed = new List<Vector3Int>();
        timeBtwActions = startTimeAction;
        
    }
    private void FixedUpdate()
    {
        AllEnemyActions();
        floorText.text = "Floor: " + actualFloor;
    }

    public void Init()
    {
        player.transform.position = Vector3.zero;
        SetEndPosition();
        for (int i = 0; i <= howManyEnemies; i++)
        {
            SpawnObjectAtRandom(enemiesAvailable[2], "enemy");
        }
        for(int i=0;i <=40; i++)
        {
            SpawnObjectAtRandom(obstacleAvailable[Random.Range(0, obstacleAvailable.Count)], "obstacle");
        }
        for(int i=0;i<=Random.Range(5, 10); i++)
        {
            SpawnObjectAtRandom(treasureChest, "obstacle");
        }
    
    }

    public void SetEndPosition(){
        pathFinding.MapDistance(new Vector3Int((int)player.transform.position.x, (int)player.transform.position.y, 0));

        int farNum = 0;
        TileType farTile = tileMap.GetGameTile(player.transform.position);

        foreach(KeyValuePair<Vector3, TileType> tile in tileMap.tiles){
            if(tile.Value.gCost> farNum){
                farTile = tile.Value;
                farNum = tile.Value.gCost;
            }
        }
        Debug.Log("Tile X: "+ farTile.x +" Tile Y: "+ farTile.y);
        tileMap.AddTile(farTile.x, farTile.y,tileMap.tileTypes[9],0);
        positionsUsed.Add(new Vector3Int(farTile.x, farTile.y, 0));
        
    }

    private void AllEnemyActions()
    {
        if (!playerTurn)
        {
            int i=0;
            while(i<=enemiesSpawned.Count-1)
            { 
                enemiesSpawned[i].GetComponent<Enemy>().thisEnemyAction = true;
                
                /*
                while (enemiesSpawned[i].GetComponent<Enemy>().thisEnemyAction)
                {

                }*/
                i++;
            }
            playerTurn = true;
            
        }
    }


    public static void Exit()
    {
        Application.Quit();
    }
    void SpawnObjectAtRandom(GameObject go, string type)
    {
        Vector3Int randPos = tileMap.floorList[Random.Range(0, tileMap.floorList.Count - 1)];
        GameObject insta = null;
        if (!positionsUsed.Contains(randPos) && randPos != player.transform.position)
        {
            insta = Instantiate(go, randPos, Quaternion.identity);
            positionsUsed.Add(randPos);
            switch (type)
            {
                case "enemy":
                    enemiesSpawned.Add(insta);
                    insta.GetComponent<Enemy>().player = player;
                    insta.GetComponent<Enemy>().tileX = randPos.x;
                    insta.GetComponent<Enemy>().tileY = randPos.y;
                    insta.GetComponent<Enemy>().pathfinding = pathFinding;
                    break;
                case "obstacle":
              
                    break;

            }
        }
        else
        {
            SpawnObjectAtRandom(go, type);
        }
        
        


            
    }
    private void OnDestroy()
    {
        if (isDead)
        {
            PlayerPrefs.DeleteKey("actualFloor");
            return;
        }
        if (actualFloor != 6)
        {
            PlayerPrefs.SetInt("actualFloor", actualFloor);
            return;
        }

  
      
        

    }



}
