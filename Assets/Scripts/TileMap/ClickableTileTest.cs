using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableTileTest : MonoBehaviour
{
    public int tileX;
    public int tileY;
    public TileMapTest map;
    public Enemy enemyOnTile;
    public Player playerOnTile;
    public PathFinding pathFinding;

    public bool isSomethingOnTile;


    private void Start()
    {
        pathFinding = PathFinding.instance;
   
    }
    private void FixedUpdate()
    {
        if (!isSomethingOnTile)
        {
            map.ChangeWalkTile(new Vector3(tileX, tileY, 0), true);
        }

    }
    private void OnMouseUp()
    {
        if (IsPointerOverUIObject())
        {
            return;
        }
        Debug.Log("Click" + tileX + " " + tileY);
     
        map.PlayerGeneratePathTo(tileX, tileY);
        


          
    }
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.gameObject.tag == "Enemy")
        {
            isSomethingOnTile = true;
            enemyOnTile = collision.gameObject.GetComponent<Enemy>();
            map.ChangeWalkTile(new Vector3(tileX, tileY, 0), false);
        }
        if(collision.gameObject.tag == "Obstacle")
        {
            isSomethingOnTile = true;
        }
       

    }
    public bool IsPlayerInTile()
    {
        return playerOnTile;
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Enemy")
        {
            isSomethingOnTile = false;
            enemyOnTile = null;
            map.ChangeWalkTile(new Vector3(tileX, tileY, 0), true);
        }
        if (collision.gameObject.tag == "Obstacle")
        {
            isSomethingOnTile = false;
        }

    }
    
}
