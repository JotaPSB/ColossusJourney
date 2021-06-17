using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    public PathFinding pathfinding;
    public GameObject player;
    public LayerMask whatIsPlayer;
    public List<Node> currentPath = new List<Node>();
    public TileMapTest map;
    private Player playerS;
    public TileType currentTile;
    public List<TileType> neighTiles;
    public GameObject floatingDamage;
    public float timeBtwMove;
    public float startTime;
    public int tileX;
    public int tileY;
    public string name;
    public int maxHealth;
    public int minDamage;
    public int maxDamage;
    public bool thisEnemyAction;
    public bool enemyDead =false;

    public GameObject throwable;


    public GameManager gm;
    public EnemyStats enemyStats;
    public string jsonSave;

    private void Awake()
    {
        jsonSave = PlayerPrefs.GetString("enemyStats"+name, "null");
        if(jsonSave == "null")
        {
            enemyStats = new EnemyStats(maxHealth, minDamage, maxDamage);
        }
        else
        {
            enemyStats = JsonUtility.FromJson<EnemyStats>(jsonSave);
        }
        
    }


    void Start()
    {
        gm = GameManager.instance;
        pathfinding = PathFinding.instance;
        player= gm.player;
        map = TileMapTest.instance;
        thisEnemyAction = false;
        startTime = 1;
        currentTile = map.GetGameTile(new Vector3(tileX, tileY, 0));
       

    }
    // Update is called once per frame
    void Update()
    {
        if (enemyStats.currentHealth <= 0)
        {
           gm.enemiesSpawned.Remove(gameObject);
           enemyDead = true;
           Destroy(gameObject);

        }
        EnemyTurn();

    }

    private void OnMouseUp()
    {
       
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        Collider2D[] searchForPlayer = Physics2D.OverlapCircleAll(transform.position, 1.2f, whatIsPlayer);
        if (!gm.isPlayerThrowing)
        {
            searchForPlayer = Physics2D.OverlapCircleAll(transform.position, 1.2f, whatIsPlayer);
            foreach (Collider2D tile in searchForPlayer)
            {

                Damage(tile.gameObject.GetComponent<Player>().Attack());
                Debug.Log("Enemy health: " + enemyStats.currentHealth);
                GameManager.instance.playerTurn = false;
            }
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(player.transform.position, transform.position);
            if(hit.collider.gameObject.tag == "Wall" || hit.collider.gameObject.tag == "Obstacle")
            {
                return;
            }

            throwable.GetComponent<SpriteRenderer>().sprite = gm.objectPlayerIsThrowing.icon;
            GameObject throwTransform = Instantiate(throwable, player.transform.position, Quaternion.identity);
            Vector3 shootDir = (transform.position - player.transform.position).normalized;
            throwTransform.GetComponent<ThrowingObject>().Setup(shootDir);
            player.GetComponent<Inventory>().Remove(gm.objectPlayerIsThrowing);
            gm.isPlayerThrowing = false;
            gm.adviceText.SetActive(false);
        }
    }
    public void Damage(int damage)
    {
        enemyStats.currentHealth -= damage;
        floatingDamage.GetComponentInChildren<TextMesh>().text = damage.ToString();
        Instantiate(floatingDamage, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
    }
    /*
    void MoveTowards(Vector3 targetPos)
    {
        List<TileType> path = pathfinding.FindPath((int)transform.position.x, (int)transform.position.y, (int)targetPos.x, (int)targetPos.y);
        //Debug.Log("Hasta aqui");
        Debug.Log(path[1].x + ", " + path[1].y);
        transform.position = new Vector3(path[1].x, path[1].y, 0);
    }*/
    public void MoveNextTile()
    {
        
        if (currentPath == null)
        {
            return;
        }
                
        tileX = currentPath[1].x;
        tileY = currentPath[1].y;
        Debug.Log(tileX + " " + tileY);
       

        currentPath.RemoveAt(0);
        transform.position = map.TileCoordToWorldCoord(tileX, tileY);
        currentTile = map.GetGameTile(new Vector3(tileX, tileY, 0));
        if (currentPath.Count == 1)
        {
            currentPath = null;
        }
        
       
    }
    public void EnemyTurn()
    {

        if (PlayerDetected() && thisEnemyAction)
        {
            if (!PlayerInRange())
            {
                currentPath = map.GeneratePathTo((int)player.transform.position.x, (int)player.transform.position.y, gameObject.transform);
                MoveNextTile();
                
            }
            
        }
        thisEnemyAction = false;

        StartCoroutine(WaitUntilEnemyFinishes());

        
    }
 
    public bool PlayerInRange()
    {
        Collider2D[] searchForPlayer = Physics2D.OverlapCircleAll(transform.position, 1.2f, whatIsPlayer);
        foreach(Collider2D tile in searchForPlayer)
        {
            playerS = tile.gameObject.GetComponent<Player>();
                
            playerS.Damage(Random.Range(enemyStats.minDamage, enemyStats.maxDamage+1));
            tile.gameObject.GetComponent<Unit>().StopMovement();
            return true;
        }
       
        return false;
    }


    public bool PlayerDetected()
    {
        Collider2D[] detectingPlayer = Physics2D.OverlapCircleAll(transform.position, 4f, whatIsPlayer);
        foreach (Collider2D tile in detectingPlayer)
        {
            if (tile.gameObject.tag == "Player")
            {
                playerS = tile.gameObject.GetComponent<Player>();
                return true;
            }
     
           
        }
        return false;
    }
     

    IEnumerator WaitUntilEnemyFinishes()
    {
        yield return new WaitForSeconds(1f);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.85f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 4f);
    }
    private void OnDestroy()
    {
        if (gm.isDead)
        {
            PlayerPrefs.DeleteKey("enemyStats"+ name);
            return;
        }
        if (enemyDead)
        {
            return;
        }
        if (gm.actualFloor != 6)
        {
            enemyStats.setMaxHealth(enemyStats.GetMaxHealth() + 15);
            enemyStats.setDamage(enemyStats.GetMaxDamage() + 2);
            PlayerPrefs.SetString("enemyStats" + name, JsonUtility.ToJson(enemyStats));
            return;
        }
    }
    
}
