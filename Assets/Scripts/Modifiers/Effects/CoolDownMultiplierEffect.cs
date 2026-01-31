using UnityEngine;

/// <summary>
/// Revertible effect that modifies magic cooldown
/// </summary>
public class CooldownMultiplierEffect : IRevertibleEffect
{
    private readonly float multiplier;
    private float previousMultiplier;
    private MagicSystem magicSystem;

    // Interface implementation - AÑADIDO
    public string DebugName => $"CooldownMultiplier({multiplier:F2}x)";

    public CooldownMultiplierEffect(float multiplier)
    {
        this.multiplier = multiplier;
    }

    /// <summary>
    /// Apply effect by storing current multiplier and applying new one
    /// </summary>
    public void Apply(GameManager gameManager)
    {
        // Get MagicSystem from player
        if (gameManager.Player == null)
        {
            Debug.LogError("CooldownMultiplierEffect: Player is null!");
            return;
        }

        magicSystem = gameManager.Player.GetComponent<MagicSystem>();
        if (magicSystem == null)
        {
            Debug.LogError("CooldownMultiplierEffect: MagicSystem not found on Player!");
            return;
        }

        // Store current multiplier
        previousMultiplier = magicSystem.GetCooldownMultiplier();

        // Apply new multiplier (multiplicative stacking)
        float newMultiplier = previousMultiplier * multiplier;
        magicSystem.SetCooldownMultiplier(newMultiplier);

        Debug.Log($"<color=cyan>[CooldownEffect]</color> Applied {multiplier}x: {previousMultiplier:F2} → {newMultiplier:F2}");
    }

    /// <summary>
    /// Revert effect by restoring previous multiplier
    /// </summary>
    public void Revert(GameManager gameManager)
    {
        if (magicSystem == null)
        {
            Debug.LogWarning("CooldownMultiplierEffect: MagicSystem is null on revert");
            return;
        }

        // Divide out this effect's multiplier
        float currentMultiplier = magicSystem.GetCooldownMultiplier();
        float restoredMultiplier = currentMultiplier / multiplier;
        magicSystem.SetCooldownMultiplier(restoredMultiplier);

        Debug.Log($"<color=yellow>[CooldownEffect]</color> Reverted {multiplier}x: {currentMultiplier:F2} → {restoredMultiplier:F2}");
    }
}