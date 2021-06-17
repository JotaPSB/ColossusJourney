using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public static PlayerStats instance;
    public int maxHealth = 100;
    public int currentHealth;
    public int damage = 20;
   
    // Start is called before the first frame update

    public PlayerStats()
    {
        currentHealth = maxHealth;
        instance = this;
     
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }




}
