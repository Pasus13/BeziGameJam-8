using System.Collections.Generic;
using System.Linq;

public class ModifierOffer
{
    public List<ModifierData> Mods { get; } = new List<ModifierData>();

    public string Title => string.Join(" + ", Mods.Select(m => m.Title));
    public string Description => string.Join("\n", Mods.Select(m => m.Description));

    public void Apply(GameManager gm)
    {
        foreach (var mod in Mods)
            mod.Apply(gm);
    }
}

