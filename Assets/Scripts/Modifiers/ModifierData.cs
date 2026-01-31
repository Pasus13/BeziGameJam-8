using Mono.Cecil;
using UnityEngine;

public abstract class ModifierData : ScriptableObject
{
    public enum ModifierType { Good, Bad}

    public ModifierType Type;

    public string Title;
    [TextArea] public string Description;

    public abstract IRevertibleEffect Apply(GameManager gameManager);
}


