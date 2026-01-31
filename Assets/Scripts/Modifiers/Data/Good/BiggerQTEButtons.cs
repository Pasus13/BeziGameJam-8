using UnityEngine;

/// <summary>
/// Good Modifier: Increases QTE perfect zone size by a configurable percentage
/// Makes it easier to hit perfect timing
/// </summary>
[CreateAssetMenu(menuName = "Modifiers/Good/BiggerQTEButtons")]
public class BiggerQTEButtons : ModifierData
{
    [Header("QTE Zone Increase")]
    [Tooltip("Percentage of perfect zone size increase (0-200). Example: 30 = 30% bigger zone")]
    [Range(0f, 200f)]
    [SerializeField] private float zoneIncreasePercentage = 30f;

    [Header("Optional: Good Zone")]
    [Tooltip("Also increase good zone? (usually not needed)")]
    [SerializeField] private bool alsoIncreaseGoodZone = false;
    [Range(0f, 200f)]
    [SerializeField] private float goodZoneIncreasePercentage = 20f;

    public override IRevertibleEffect Apply(GameManager gameManager)
    {
        if (gameManager == null)
        {
            Debug.LogError("BiggerQTEButtons: GameManager is null!");
            return null;
        }

        // Convert percentage to multiplier
        // 30% increase = 1.3 multiplier (100 + 30 = 130%)
        float perfectMultiplier = 1f + (zoneIncreasePercentage / 100f);
        float goodMultiplier = alsoIncreaseGoodZone ? 1f + (goodZoneIncreasePercentage / 100f) : 1f;

        // Create the effect
        var effect = new QTEZoneMultiplierEffect(perfectMultiplier, goodMultiplier);

        // Apply it
        effect.Apply(gameManager);

        Debug.Log($"<color=green>[BiggerQTEButtons]</color> Applied: {Title} ({zoneIncreasePercentage}% bigger = {perfectMultiplier:F2}x)");

        // Return the effect so it can be reverted later
        return effect;
    }
}
