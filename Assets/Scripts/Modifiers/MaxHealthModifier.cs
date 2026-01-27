using UnityEngine;

public class MaxHealthModifier : IRevertibleEffect
{
    private float healthMultiplier;
    private int previousMaxHealth;

    public string DebugName => $"MaxHealth x{healthMultiplier}";

    public MaxHealthModifier(float multiplier)
    {
        healthMultiplier = multiplier;
    }

    public string GetDescription()
    {
        int percentage = Mathf.RoundToInt((healthMultiplier - 1f) * 100f);
        return $"+{percentage}% Max Health";
    }

    public void Apply(GameManager gameManager)
    {
        HealthComponent playerHealth = gameManager.GetComponent<HealthComponent>();
        if (playerHealth != null)
        {
            previousMaxHealth = playerHealth.MaxHealth;
            playerHealth.ModifyMaxHealth(healthMultiplier);
            Debug.Log($"Applied Max Health Modifier: x{healthMultiplier}");
        }
    }

    public void Revert(GameManager gameManager)
    {
        HealthComponent playerHealth = gameManager.GetComponent<HealthComponent>();
        if (playerHealth != null)
        {
            float revertMultiplier = (float)previousMaxHealth / playerHealth.MaxHealth;
            playerHealth.ModifyMaxHealth(revertMultiplier);
            Debug.Log($"Reverted Max Health Modifier");
        }
    }
}
