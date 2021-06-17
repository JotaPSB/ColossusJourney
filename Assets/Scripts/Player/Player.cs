using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    PlayerStats playerStats;
    private string jsonSave;
   
    public Text health;
    public GameObject floatingDamage;
    

    // Start is called before the first frame update
    void Awake()
    {
        jsonSave = PlayerPrefs.GetString("playerStatsSave", "null");
        if(jsonSave == "null")
        {
            playerStats = new PlayerStats();
        }
        else
        {
            playerStats = JsonUtility.FromJson<PlayerStats>(jsonSave);
        }
        health.text = "Health: " + playerStats.currentHealth;

        //uiInventory.SetInventory(playerStats.GetInventory());
        
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        IsDead();
        health.text = "Health: " + playerStats.currentHealth;
        if (Input.GetKeyDown(KeyCode.H))
        {
            playerStats.currentHealth = playerStats.maxHealth;
        }
    }

    public void Damage(int damage)
    {
        playerStats.currentHealth -= damage;
        floatingDamage.GetComponentInChildren<TextMesh>().text = damage.ToString() ;
        Instantiate(floatingDamage, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
        
        Debug.Log("Position "+transform.position.x +", "+transform.position.y);
    }
    public int Attack()
    {
        return playerStats.damage;
    }
    public void IsDead()
    {
        if (playerStats.currentHealth <= 0)
        {
            
           
            GameManager.instance.isDead = true;

            SceneManager.LoadScene(2);
            
            
        }

    }
    void OnDestroy()
    {
        if (!GameManager.instance.isDead)
        {
            PlayerPrefs.SetString("playerStatsSave", JsonUtility.ToJson(playerStats));
        }
        else
        {
            PlayerPrefs.DeleteKey("playerStatsSave");
        
        }
    }
}
