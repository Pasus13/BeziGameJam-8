using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AutoFixQTESpritesOnce
{
    private const string PREF_KEY = "QTESprites_AutoFixed_v1";

    static AutoFixQTESpritesOnce()
    {
        if (!EditorPrefs.GetBool(PREF_KEY, false))
        {
            EditorApplication.delayCall += () =>
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    FixQTESpritesEditor.FixSprites();
                    EditorPrefs.SetBool(PREF_KEY, true);
                    Debug.Log("<color=green>[AutoFixQTESprites]</color> Auto-fix completed. This will not run again.");
                }
            };
        }
    }
}
