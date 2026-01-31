using UnityEngine;

/// <summary>
/// Bad Modifier: Increases magic cooldown by 50%
/// </summary>
[CreateAssetMenu(menuName = "Modifiers/Bad/IncreaseMagicCooldown")]
public class IncreaseMagicCooldown : ModifierData
{
    [Range(0f, 200f)]
    [SerializeField] private float increasePercentage = 50f;

    public override IRevertibleEffect Apply(GameManager gameManager)
    {
        if (gameManager == null)
        {
            Debug.LogError("SlowRechargeModifier: GameManager is null!");
            return null;
        }

        float multiplier = 1f + (increasePercentage / 100f);
        // Create the effect
        var effect = new CooldownMultiplierEffect(multiplier);

        // Apply it
        effect.Apply(gameManager);

        Debug.Log($"<color=red>[SlowRecharge]</color> Applied: {Title}");

        // Return the effect so it can be reverted later
        return effect;
    }
}
