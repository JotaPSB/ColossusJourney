using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour
{
    public int tileX;
    public int tileY;
    public TileMapTest map;
   

    private void OnMouseUp()
    {
        Debug.Log("Click" + tileX + " " + tileY);

        map.PlayerGeneratePathTo(tileX, tileY);
    }
}
