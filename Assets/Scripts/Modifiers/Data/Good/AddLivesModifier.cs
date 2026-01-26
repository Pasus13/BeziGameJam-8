using UnityEngine;

[CreateAssetMenu(menuName = "Modifiers/Good/Add Life")]
public class AddLifeModifier : ModifierData
{
    public int lives = 1;

    public override IRevertibleEffect Apply(GameManager gameManager)
    {
        // Busca el HealthSystem en la escena
        HealthSystem health = Object.FindFirstObjectByType<HealthSystem>();

        if (health == null)
        {
            Debug.LogError("AddLifeModifier: No se encontró HealthSystem en la escena.");
            return null;
        }

        health.AddLifes(lives);
        return null; // No reversible
    }
}



