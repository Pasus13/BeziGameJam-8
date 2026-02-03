using UnityEngine;

[CreateAssetMenu(menuName = "Modifiers/Good/Increase Max Health")]
public class MaxHealthModifier : ModifierData
{
    [Header("Health Increase")]
    [Range(1f, 200f)]
    [Tooltip("Percentage of max health increase (e.g.: 25 = +25% health)")]
    public float percent = 25f;

    public override IRevertibleEffect Apply(GameManager gameManager)
    {
        if (gameManager == null) return null;

        // Get PlayerHealth from GameManager
        PlayerHealth playerHealth = gameManager.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("MaxHealthModifier: PlayerHealth not found in GameManager.");
            return null;
        }

        // Calculate multiplier (25% = 1.25x)
        float mult = 1f + percent / 100f;

        // Apply modifier
        playerHealth.ModifyMaxHealth(mult);

        Debug.Log($"MaxHealthModifier: Applied x{mult:F2} ({percent}%)");

        // Return the effect to be able to revert it later
        return new MaxHealthEffect(mult);
    }

    private class MaxHealthEffect : IRevertibleEffect
    {
        private readonly float _mult;

        public string DebugName => $"MaxHealth x{_mult:F2}";

        public MaxHealthEffect(float mult) => _mult = mult;

        public void Revert(GameManager gameManager)
        {
            if (gameManager == null) return;

            PlayerHealth playerHealth = gameManager.GetComponent<PlayerHealth>();
            if (playerHealth == null) return;

            // Revert by applying the inverse multiplier
            playerHealth.ModifyMaxHealth(1f / _mult);

            Debug.Log($"MaxHealthModifier: Reverted x{_mult:F2}");
        }
    }
}