using UnityEngine;

[CreateAssetMenu(menuName = "Modifiers/Offer Recipe")]
public class OfferRecipe : ScriptableObject
{
    [Header("Counts")]
    public int goodCount = 1;
    public int badCount = 1;
    public int neutralCount = 0;
}
