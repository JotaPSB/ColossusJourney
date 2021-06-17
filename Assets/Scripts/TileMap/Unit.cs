using System.Threading;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Unit : MonoBehaviour
{
    float speed = 450f;
    public int tileX;
    public int tileY;
    public TileMapTest map;
    public TileType currentTile;
    public float timeBtwMove;
    public float startTimeBtwMove;
    public List<Node> currentPath = null;
    public GameObject mapCreator;


    private void Start()
    {
        startTimeBtwMove = 0.10f;
        StartCoroutine(CoUpdate());
    }
    /*
    private void Update() {
   

    }*/
    IEnumerator CoUpdate()
    {
        WaitForSeconds wait = new WaitForSeconds(startTimeBtwMove);
        while (true)
        {
            if (currentPath != null)
            {
                int currNode = 0;
                while (currNode < currentPath.Count - 1)
                {
                    Vector3 start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) + new Vector3(0, 0, 1f);
                    Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y) + new Vector3(0, 0, 1f);

                    Debug.DrawLine(start, end, Color.red);
                    currNode++;
                }

                if (GameManager.instance.playerTurn)
                {
                    MoveNextTile();
                    if (GameManager.instance.enemiesSpawned.Count > 0)
                    {

                        yield return wait;
                        GameManager.instance.playerTurn = false;
                    }
                    
                }



            }

            yield return null;
        }
        
    }
    public void MoveNextTile(){
       
        if(currentPath== null){
            return;
        }
        tileX =(int) currentPath[1].x;
        tileY = (int)currentPath[1].y;
        currentPath.RemoveAt(0);
        //Debug.Log(tileX + " " + tileY);
        transform.position =  map.TileCoordToWorldCoord(tileX,tileY);
        //transform.position = Vector3.MoveTowards(transform.position, map.TileCoordToWorldCoord(tileX, tileY), speed * Time.deltaTime);
        currentTile = map.GetGameTile(new Vector3(tileX, tileY, 0));
        if(currentTile.name == "Stairs")
        {
            transform.position = Vector3.zero;
            GameManager.instance.actualFloor++;
            if (GameManager.instance.actualFloor != 6)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                SceneManager.LoadScene(2);
            }
            
        }

      
        if(currentPath.Count == 1){
            currentPath = null;
        }
    }
    public void StopMovement()
    {
        currentPath = null;
    }

}
