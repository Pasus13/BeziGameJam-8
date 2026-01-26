using UnityEngine;

[CreateAssetMenu(menuName = "Modifiers/Good/Increase Speed")]
public class IncreaseSpeedModifier : ModifierData
{
    [Range(1f, 200f)] public float percent = 25f;

    public override IRevertibleEffect Apply(GameManager gameManager)
    {
        if (gameManager == null) return null;

        var pm = gameManager.PlayerMovement;
        if (pm == null)
        {
            Debug.LogError("IncreaseSpeedModifier: No encuentro GameManager.PlayerMovement.");
            return null;
        }

        float mult = 1f + percent / 100f;

        // ⚠️ Asegúrate de que ESTE método existe en tu PlayerMovement.
        // Si no existe, cambia esta línea al método real que estés usando.
        pm.MultiplySpeed(mult);

        return new SpeedEffect(mult);
    }

    private class SpeedEffect : IRevertibleEffect
    {
        private readonly float _mult;
        public string DebugName => $"Speed x{_mult:F2}";

        public SpeedEffect(float mult) => _mult = mult;

        public void Revert(GameManager gameManager)
        {
            if (gameManager == null) return;

            var pm = gameManager.PlayerMovement;
            if (pm == null) return;

            pm.MultiplySpeed(1f / _mult);
        }
    }
}





