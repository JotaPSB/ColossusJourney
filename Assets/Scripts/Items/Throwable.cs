using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Throwable")]
public class Throwable : Item {
    public int damage;


    public override void Use()
    {
        GameManager.instance.isPlayerThrowing = true;
        GameManager.instance.objectPlayerIsThrowing = this;
        InventoryUI.instance.CloseInventory();
        InventoryUI.instance.adviceText.SetActive(true);
       
    }
}
