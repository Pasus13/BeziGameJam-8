using UnityEngine;

/// <summary>
/// Revertible effect that modifies QTE zone sizes
/// </summary>
public class QTEZoneMultiplierEffect : IRevertibleEffect
{
    private readonly float perfectZoneMultiplier;
    private readonly float goodZoneMultiplier;
    private float previousPerfectMultiplier;
    private float previousGoodMultiplier;

    // Interface implementation
    public string DebugName => $"QTEZone(Perfect:{perfectZoneMultiplier:F2}x, Good:{goodZoneMultiplier:F2}x)";

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="perfectMultiplier">Multiplier for perfect zone size</param>
    /// <param name="goodMultiplier">Multiplier for good zone size (optional, default 1.0)</param>
    public QTEZoneMultiplierEffect(float perfectMultiplier, float goodMultiplier = 1f)
    {
        this.perfectZoneMultiplier = perfectMultiplier;
        this.goodZoneMultiplier = goodMultiplier;
    }

    /// <summary>
    /// Apply effect by storing current multipliers and applying new ones
    /// </summary>
    public void Apply(GameManager gameManager)
    {
        if (QTEManager.Instance == null)
        {
            Debug.LogError("QTEZoneMultiplierEffect: QTEManager.Instance is null!");
            return;
        }

        // Store current multipliers
        previousPerfectMultiplier = QTEManager.Instance.GetPerfectZoneMultiplier();
        previousGoodMultiplier = QTEManager.Instance.GetGoodZoneMultiplier();

        // Apply new multipliers (multiplicative stacking)
        float newPerfectMultiplier = previousPerfectMultiplier * perfectZoneMultiplier;
        float newGoodMultiplier = previousGoodMultiplier * goodZoneMultiplier;

        QTEManager.Instance.SetPerfectZoneMultiplier(newPerfectMultiplier);
        QTEManager.Instance.SetGoodZoneMultiplier(newGoodMultiplier);

        Debug.Log($"<color=cyan>[QTEZoneEffect]</color> Applied Perfect:{perfectZoneMultiplier:F2}x ({previousPerfectMultiplier:F2} → {newPerfectMultiplier:F2})");
    }

    /// <summary>
    /// Revert effect by restoring previous multipliers
    /// </summary>
    public void Revert(GameManager gameManager)
    {
        if (QTEManager.Instance == null)
        {
            Debug.LogWarning("QTEZoneMultiplierEffect: QTEManager.Instance is null on revert");
            return;
        }

        // Divide out this effect's multipliers
        float currentPerfectMultiplier = QTEManager.Instance.GetPerfectZoneMultiplier();
        float currentGoodMultiplier = QTEManager.Instance.GetGoodZoneMultiplier();

        float restoredPerfectMultiplier = currentPerfectMultiplier / perfectZoneMultiplier;
        float restoredGoodMultiplier = currentGoodMultiplier / goodZoneMultiplier;

        QTEManager.Instance.SetPerfectZoneMultiplier(restoredPerfectMultiplier);
        QTEManager.Instance.SetGoodZoneMultiplier(restoredGoodMultiplier);

        Debug.Log($"<color=yellow>[QTEZoneEffect]</color> Reverted Perfect:{perfectZoneMultiplier:F2}x ({currentPerfectMultiplier:F2} → {restoredPerfectMultiplier:F2})");
    }
}
