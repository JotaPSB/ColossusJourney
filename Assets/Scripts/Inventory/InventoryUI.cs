using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/* This object manages the inventory UI. */

public class InventoryUI : MonoBehaviour {

	public GameObject inventoryUI;	// The entire UI
	public Transform itemsParent;   // The parent object of all the items
	InventorySlot[] slots;
	Inventory inventory;    // Our current inventory
	public static InventoryUI instance;
	public GameObject adviceText;

	void Start ()
	{
		inventory = Inventory.instance;
		inventory.onItemChangedCallback += UpdateUI;
		slots = itemsParent.GetComponentsInChildren<InventorySlot>();
		instance = this;
	}

	



	public void OpenInventory()
    {
		inventoryUI.SetActive(!inventoryUI.activeSelf);
        if (GameManager.instance.isPlayerThrowing)
        {
			GameManager.instance.isPlayerThrowing = false;
			GameManager.instance.adviceText.SetActive(false);
		}
		UpdateUI();
	}
	public void CloseInventory()
    {
		inventoryUI.SetActive(!inventoryUI.activeSelf);
	}
	public void UpdateUI()
	{
		

		for (int i = 0; i < slots.Length; i++)
		{
			if (i < inventory.items.Count)
			{
				slots[i].AddItem(inventory.items[i]);
			}
			else
			{
				slots[i].ClearSlot();
			}
		}
	}

}
