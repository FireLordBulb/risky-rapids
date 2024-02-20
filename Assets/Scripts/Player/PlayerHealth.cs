using System;

public class PlayerHealth
{
    private float currentHealth;
    private float maxHealth;
    private float armor;
    private float currentMaxArmor;
    
    public PlayerHealth(float maxHealth, float armor)
    {
        this.maxHealth = maxHealth;
        currentHealth = this.maxHealth;
        this.armor = armor;
        currentMaxArmor = armor;
    }

    public void ReplenishHealth()
    {
        currentHealth = maxHealth;
        armor = currentMaxArmor;
        UIManager.Instance.UpdateHealthSlider(currentHealth);
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
        UIManager.Instance.UpdateHealthSlider(currentHealth);
        
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
