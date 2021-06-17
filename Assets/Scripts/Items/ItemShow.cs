using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShow : MonoBehaviour
{
    public Item consumableData;
    public Inventory inventory;

    private void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = consumableData.icon;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Inventory>().Add(consumableData);
        }
    }
}
