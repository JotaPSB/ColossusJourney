using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class EnemyStats {
    
    public int maxHealth;
    public int currentHealth;
    public int minDamage;
    public int maxDamage;
    public EnemyStats(int maxHealth, int minDamage, int maxDamage)
    {
      
        this.maxHealth = maxHealth;
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
        currentHealth = this.maxHealth;
    }

    public void setMaxHealth(int maxHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
    }

    public void setDamage(int maxDamage)
    {
      
        this.maxDamage = maxDamage;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
    public int GetMaxDamage()
    {
        return maxDamage;
    }


}
