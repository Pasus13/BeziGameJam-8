using System.Collections.Generic;
using UnityEngine;
using static ModifierData;

public class ModifierManager : MonoBehaviour
{
    [Header("All atomic modifiers")]
    [SerializeField] private List<ModifierData> allModifiers = new List<ModifierData>();

    [Header("Offer recipes for the 3 buttons")]
    [SerializeField] private OfferRecipe[] recipesForButtons = new OfferRecipe[3];

    private readonly List<ModifierData> _good = new();
    private readonly List<ModifierData> _bad = new();
    private readonly List<ModifierData> _neutral = new();

    // Runtime active effects
    private readonly List<IRevertibleEffect> _activeEffects = new();
    public IReadOnlyList<IRevertibleEffect> ActiveEffects => _activeEffects;

    private void Awake()
    {
        BuildPools();
    }

    #region Pool Building

    private void BuildPools()
    {
        _good.Clear();
        _bad.Clear();
        _neutral.Clear();

        foreach (var m in allModifiers)
        {
            if (m == null) continue;

            switch (m.Type)
            {
                case ModifierType.Good:
                    _good.Add(m);
                    break;
                case ModifierType.Bad:
                    _bad.Add(m);
                    break;
            }
        }
    }

    #endregion

    #region Offer Generation

    public ModifierOffer[] GenerateOffers(int buttonCount = 3)
    {
        if (recipesForButtons == null || recipesForButtons.Length < buttonCount)
        {
            Debug.LogError("ModifierManager: recipesForButtons doesn't have enough recipes.");
            return new ModifierOffer[0];
        }

        // Temporary copies to avoid repetition within the same generation
        var goodPool = new List<ModifierData>(_good);
        var badPool = new List<ModifierData>(_bad);
        var neutralPool = new List<ModifierData>(_neutral);

        ModifierOffer[] offers = new ModifierOffer[buttonCount];

        for (int i = 0; i < buttonCount; i++)
        {
            OfferRecipe recipe = recipesForButtons[i];
            offers[i] = BuildOffer(recipe, goodPool, badPool, neutralPool);
        }

        return offers;
    }

    private ModifierOffer BuildOffer(
        OfferRecipe recipe,
        List<ModifierData> goodPool,
        List<ModifierData> badPool,
        List<ModifierData> neutralPool)
    {
        ModifierOffer offer = new ModifierOffer();

        PickMany(recipe.goodCount, goodPool, offer.Mods, "Good");
        PickMany(recipe.badCount, badPool, offer.Mods, "Bad");
        PickMany(recipe.neutralCount, neutralPool, offer.Mods, "Neutral");

        return offer;
    }

    private void PickMany(int count, List<ModifierData> from, List<ModifierData> to, string label)
    {
        if (count <= 0) return;

        if (from.Count < count)
        {
            Debug.LogWarning(
                $"ModifierManager: not enough {label} modifiers. " +
                $"Requested {count}, available {from.Count}."
            );
            count = from.Count; // safe degradation
        }

        for (int i = 0; i < count; i++)
        {
            int r = Random.Range(0, from.Count);
            to.Add(from[r]);
            from.RemoveAt(r);
        }
    }

    #endregion

    #region Apply Modifiers

    /// <summary>
    /// Applies all modifiers from an offer and saves their active effects.
    /// Returns the handles (IRevertibleEffect) created by this offer.
    /// </summary>
    public List<IRevertibleEffect> ApplyOffer(ModifierOffer offer, GameManager gameManager)
    {
        var appliedEffects = new List<IRevertibleEffect>();

        if (offer == null || gameManager == null)
        {
            Debug.LogError("ModifierManager.ApplyOffer: offer or gameManager is null.");
            return appliedEffects;
        }

        if (offer.Mods == null || offer.Mods.Count == 0)
        {
            Debug.LogWarning("ModifierManager.ApplyOffer: the offer doesn't contain modifiers.");
            return appliedEffects;
        }

        foreach (var mod in offer.Mods)
        {
            if (mod == null) continue;

            IRevertibleEffect effect = mod.Apply(gameManager);
            if (effect != null)
            {
                _activeEffects.Add(effect);
                appliedEffects.Add(effect);
            }
        }

        return appliedEffects;
    }

    #endregion

    #region Revert Effects

    /// <summary>
    /// Reverts a specific effect by index.
    /// </summary>
    public bool RevertEffectAt(int index, GameManager gameManager)
    {
        if (gameManager == null)
        {
            Debug.LogError("ModifierManager.RevertEffectAt: gameManager is null.");
            return false;
        }

        if (index < 0 || index >= _activeEffects.Count)
        {
            Debug.LogWarning($"ModifierManager.RevertEffectAt: invalid index {index}.");
            return false;
        }

        _activeEffects[index].Revert(gameManager);
        _activeEffects.RemoveAt(index);
        return true;
    }

    /// <summary>
    /// Reverts a specific effect using its reference (handle).
    /// </summary>
    public bool RevertEffect(IRevertibleEffect effect, GameManager gameManager)
    {
        if (effect == null || gameManager == null) return false;

        int index = _activeEffects.IndexOf(effect);
        if (index < 0) return false;

        effect.Revert(gameManager);
        _activeEffects.RemoveAt(index);
        return true;
    }

    /// <summary>
    /// Reverts all active effects (run / level reset).
    /// </summary>
    public void RevertAll(GameManager gameManager)
    {
        if (gameManager == null)
        {
            Debug.LogError("ModifierManager.RevertAll: gameManager is null.");
            return;
        }

        // Important: revert in reverse order
        for (int i = _activeEffects.Count - 1; i >= 0; i--)
        {
            _activeEffects[i].Revert(gameManager);
        }

        _activeEffects.Clear();
    }

    #endregion
}
