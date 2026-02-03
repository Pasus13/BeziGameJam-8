using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class FixQTESpritesEditor
{
    [MenuItem("Tools/Fix QTE Sprites")]
    public static void FixSprites()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogWarning("<color=yellow>[FixQTESprites]</color> Cannot fix sprites during Play mode. Please exit Play mode first.");
            return;
        }

        if (SceneManager.GetActiveScene().path != "Assets/Scenes/MainMap.unity")
        {
            Debug.LogWarning("<color=yellow>[FixQTESprites]</color> Please open the MainMap scene first!");
            return;
        }

        QTEVisualizer visualizer = Object.FindFirstObjectByType<QTEVisualizer>();
        
        if (visualizer == null)
        {
            Debug.LogError("<color=red>[FixQTESprites]</color> QTEVisualizer not found in the current scene!");
            return;
        }

        SerializedObject so = new SerializedObject(visualizer);

        so.FindProperty("upArrowSprite").objectReferenceValue = LoadSprite("Assets/UI/QTE/Arriba.png", "Arriba_0");
        so.FindProperty("downArrowSprite").objectReferenceValue = LoadSprite("Assets/UI/QTE/Abajo.png", "Abajo_0");
        so.FindProperty("rightArrowSprite").objectReferenceValue = LoadSprite("Assets/UI/QTE/Derecha.png", "Derecha_0");
        so.FindProperty("leftArrowSprite").objectReferenceValue = LoadSprite("Assets/UI/QTE/Izquierda.png", "Izquierda_0");
        so.FindProperty("perfectRingSprite").objectReferenceValue = LoadSprite("Assets/UI/QTE/Aro.png", "Aro_0");
        so.FindProperty("goodRingSprite").objectReferenceValue = LoadSprite("Assets/UI/QTE/Aro.png", "Aro_0");

        so.ApplyModifiedProperties();

        EditorUtility.SetDirty(visualizer);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("<color=green>[FixQTESprites]</color> ✓ Sprites fixed and scene saved! All sprite references now point to the correct sub-sprites.");
    }

    private static Sprite LoadSprite(string assetPath, string spriteName)
    {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        
        foreach (Object asset in assets)
        {
            if (asset is Sprite sprite && sprite.name == spriteName)
            {
                return sprite;
            }
        }

        Debug.LogWarning($"<color=yellow>[FixQTESprites]</color> Sprite '{spriteName}' not found in {assetPath}");
        return null;
    }
}
