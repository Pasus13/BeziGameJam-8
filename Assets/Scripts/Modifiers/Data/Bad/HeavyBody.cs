using UnityEngine;

[CreateAssetMenu(menuName = "Modifiers/Bad/Heavy Body")]
public class HeavyBodyModifier : ModifierData
{
    [Header("Gravity Increase (%)")]
    [Range(1f, 50f)]
    public float gravityIncreasePercent = 10f;

    [Header("Optional: also increase Max Fall Speed (%)")]
    public bool alsoIncreaseMaxFallSpeed = true;

    [Range(0f, 50f)]
    public float maxFallSpeedIncreasePercent = 10f;

    public override IRevertibleEffect Apply(GameManager gameManager)
    {
        if (gameManager == null) return null;

        var pm = gameManager.PlayerMovement;
        if (pm == null)
        {
            Debug.LogError("HeavyBodyModifier: No encuentro GameManager.PlayerMovement.");
            return null;
        }

        // +10% => 1.10
        float gravityMult = 1f + gravityIncreasePercent / 100f;
        pm.MultiplyGravity(gravityMult);

        float fallMult = 1f;
        bool appliedFall = false;

        if (alsoIncreaseMaxFallSpeed && maxFallSpeedIncreasePercent > 0f)
        {
            fallMult = 1f + maxFallSpeedIncreasePercent / 100f;
            pm.MultiplyMaxFallSpeed(fallMult);
            appliedFall = true;
        }

        return new HeavyBodyEffect(gravityMult, appliedFall, fallMult);
    }

    private class HeavyBodyEffect : IRevertibleEffect
    {
        private readonly float _gravityMult;
        private readonly bool _appliedFall;
        private readonly float _fallMult;

        public string DebugName => _appliedFall
            ? $"HeavyBody grav x{_gravityMult:F2} | fall x{_fallMult:F2}"
            : $"HeavyBody grav x{_gravityMult:F2}";

        public HeavyBodyEffect(float gravityMult, bool appliedFall, float fallMult)
        {
            _gravityMult = gravityMult;
            _appliedFall = appliedFall;
            _fallMult = fallMult;
        }

        public void Revert(GameManager gameManager)
        {
            if (gameManager == null) return;

            var pm = gameManager.PlayerMovement;
            if (pm == null) return;

            // Revertimos multiplicando por el inverso
            pm.MultiplyGravity(1f / _gravityMult);

            if (_appliedFall && _fallMult != 0f)
                pm.MultiplyMaxFallSpeed(1f / _fallMult);
        }
    }
}
