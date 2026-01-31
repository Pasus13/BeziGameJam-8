using UnityEngine;

/// <summary>
/// Bad Modifier: Decreases QTE perfect zone size by a configurable percentage
/// Makes it harder to hit perfect timing
/// </summary>
[CreateAssetMenu(menuName = "Modifiers/Bad/SmallerQTEButtons")]
public class SmallerQTEButtons : ModifierData
{
    [Header("QTE Zone Reduction")]
    [Tooltip("Percentage of perfect zone size reduction (0-90). Example: 40 = 40% smaller zone")]
    [Range(0f, 90f)]
    [SerializeField] private float zoneReductionPercentage = 40f;

    [Header("Optional: Good Zone")]
    [Tooltip("Also reduce good zone? (makes it extra hard)")]
    [SerializeField] private bool alsoReduceGoodZone = false;
    [Range(0f, 50f)]
    [SerializeField] private float goodZoneReductionPercentage = 20f;

    public override IRevertibleEffect Apply(GameManager gameManager)
    {
        if (gameManager == null)
        {
            Debug.LogError("SmallerQTEButtons: GameManager is null!");
            return null;
        }

        // Convert percentage to multiplier
        // 40% reduction = 0.6 multiplier (100 - 40 = 60%)
        float perfectMultiplier = 1f - (zoneReductionPercentage / 100f);
        float goodMultiplier = alsoReduceGoodZone ? 1f - (goodZoneReductionPercentage / 100f) : 1f;

        // Create the effect
        var effect = new QTEZoneMultiplierEffect(perfectMultiplier, goodMultiplier);

        // Apply it
        effect.Apply(gameManager);

        Debug.Log($"<color=red>[SmallerQTEButtons]</color> Applied: {Title} ({zoneReductionPercentage}% smaller = {perfectMultiplier:F2}x)");

        // Return the effect so it can be reverted later
        return effect;
    }
}
