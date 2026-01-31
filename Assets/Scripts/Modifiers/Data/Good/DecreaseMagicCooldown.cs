using UnityEngine;

/// <summary>
/// Good Modifier: Reduces magic cooldown by 25%
/// </summary>
[CreateAssetMenu(menuName = "Modifiers/Good/DecreaseMagicCooldown")]
public class DecreaseMagicCooldown : ModifierData
{
    [Range(0f, 100f)]
    [SerializeField] private float reductionPercentage = 25f;

    public override IRevertibleEffect Apply(GameManager gameManager)
    {
        if (gameManager == null)
        {
            Debug.LogError("DecreaseMagicCooldown: GameManager is null!");
            return null;
        }

        float multiplier = 1f - (reductionPercentage / 100f);
        // Create the effect
        var effect = new CooldownMultiplierEffect(multiplier);

        // Apply it
        effect.Apply(gameManager);

        Debug.Log($"<color=green>[DecreaseMagicCooldown]</color> Applied: {Title}");

        // Return the effect so it can be reverted later
        return effect;
    }
}
