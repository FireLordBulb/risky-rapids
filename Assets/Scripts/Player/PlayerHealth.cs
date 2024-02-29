using System;
using UnityEngine.UI;

public class PlayerHealth
{
    private Slider healthBar;
    private float currentHealth;
    private float maxHealth;
    private float armor;
    private float currentMaxArmor;
    
    public PlayerHealth(float maxHealth, float armor, Slider healthBar)
    {
        this.maxHealth = maxHealth;
        currentHealth = this.maxHealth;
        this.armor = armor;
        currentMaxArmor = armor;
        this.healthBar = healthBar;
    }

    public void ReplenishHealth()
    {
        currentHealth = maxHealth;
        healthBar.value = currentHealth;
        armor = currentMaxArmor;
    }

    public void TakeDamage(float damage)
    {
        if (armor > 0)
        {
            armor -= damage;
            if (armor < 0)
            {
                armor *= -1;
                damage = armor;
                armor = 0;
            }
            else
                return;
        }
        
        currentHealth -= damage;
        healthBar.value = currentHealth;
        
        if (currentHealth <= 0)
        {
            GameManager.Instance.FailGame();
            ReplenishHealth();
        }
    }

    public void UpgradeArmor()
    {
        currentMaxArmor = UpgradeHolder.Instance.GetUpgradeValue(UpgradeType.Armor);
        armor = currentMaxArmor;
    }
}
