using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class TileType
{
    public string name;
    public GameObject tileVisual;
    public int x;
    public int y;

    public bool isWalkable = true;

    public float movementCost = 1;
    public int areaNumber {get; set;}

    public int gCost;
    public int hCost;
    public int fCost;

    public TileType fromTile;
    public void CalculateFCost(){
        fCost = gCost+hCost;
    }
    
}
