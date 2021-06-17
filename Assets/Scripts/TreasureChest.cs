using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour {

	

	public bool isOpen;
	public LayerMask whatIsPlayer;
	public Item[] items;


	public Sprite openChest;


    private void OnMouseUp()
    {
		Debug.Log("JEEE");
		Collider2D[] searchForPlayer;
		if (!GameManager.instance.isPlayerThrowing)
		{
			
			searchForPlayer = Physics2D.OverlapCircleAll(transform.position, 1.2f, whatIsPlayer);
			foreach (Collider2D tile in searchForPlayer)
			{
				if (tile.gameObject.tag == "Player")
				{
					Interact();
				}
			}
		}
	}
    public void Interact ()
	{
	
		if (!isOpen) {
			GetComponent<SpriteRenderer>().sprite = openChest;
			CollectTreasure ();
		}
	}

	void CollectTreasure() {

		isOpen = true;

	
		print ("Chest opened");
		Inventory.instance.Add(items[Random.Range(0, items.Length)]);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {

        }
    }
}
