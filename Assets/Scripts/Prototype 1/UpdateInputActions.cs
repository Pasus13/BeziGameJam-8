using UnityEngine;
using System.IO;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;

public static class UpdateInputActions
{
    [MenuItem("Tools/Update Input Actions for Magic System")]
    public static void UpdateMagicInputActions()
    {
        string relativePath = "Assets/Inputs/Prototype 1/PlayerInputActions.inputactions";
        string fullPath = Path.Combine(Application.dataPath, "../", relativePath);
        
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"No se encontró: {fullPath}");
            return;
        }
        
        try
        {
            string json = File.ReadAllText(fullPath);
            
            json = AddMagicAction(json, "Magic1", "aaaaaaaa-1111-1111-1111-111111111111");
            json = AddMagicAction(json, "Magic2", "bbbbbbbb-2222-2222-2222-222222222222");
            json = AddMagicAction(json, "Magic3", "cccccccc-3333-3333-3333-333333333333");
            
            json = AddMagicBinding(json, "Magic1", "aaaa1111-aaaa-1111-aaaa-111111111111", "<Keyboard>/a");
            json = AddMagicBinding(json, "Magic2", "bbbb2222-bbbb-2222-bbbb-222222222222", "<Keyboard>/s");
            json = AddMagicBinding(json, "Magic3", "cccc3333-cccc-3333-cccc-333333333333", "<Keyboard>/d");
            
            json = UpdateAttackBinding(json);
            
            File.WriteAllText(fullPath, json, Encoding.UTF8);
            
            AssetDatabase.Refresh();
            
            Debug.Log("✅ <b>Input Actions actualizados correctamente!</b>");
            Debug.Log("🔥 <color=orange>Magic1 (Fuego) → Tecla A</color>");
            Debug.Log("❄️ <color=cyan>Magic2 (Hielo) → Tecla S</color>");
            Debug.Log("⚡ <color=yellow>Magic3 (Rayo) → Tecla D</color>");
            Debug.Log("⚔️ <color=red>Attack → Tecla F</color>");
            Debug.Log("\n<b>¡Importante!</b> Si el InputManager muestra errores, abre el PlayerInputActions asset y haz clic en 'Generate C# Class'.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al actualizar Input Actions: {e.Message}");
        }
    }
    
    private static string AddMagicAction(string json, string actionName, string actionId)
    {
        if (json.Contains($"\"name\": \"{actionName}\""))
        {
            Debug.Log($"La acción {actionName} ya existe, omitiendo...");
            return json;
        }
        
        string actionTemplate = $@"                {{
                    ""name"": ""{actionName}"",
                    ""type"": ""Button"",
                    ""id"": ""{actionId}"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }},";
        
        int insertPos = json.IndexOf("\"name\": \"QTEButton1\"");
        if (insertPos > 0)
        {
            json = json.Insert(insertPos, actionTemplate + "\n                ");
        }
        
        return json;
    }
    
    private static string AddMagicBinding(string json, string actionName, string bindingId, string keyPath)
    {
        if (json.Contains($"\"action\": \"{actionName}\""))
        {
            Debug.Log($"El binding para {actionName} ya existe, omitiendo...");
            return json;
        }
        
        string bindingTemplate = $@"                {{
                    ""name"": """",
                    ""id"": ""{bindingId}"",
                    ""path"": ""{keyPath}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""{actionName}"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }},";
        
        int insertPos = json.IndexOf("\"action\": \"QTEButton1\"");
        if (insertPos > 0)
        {
            json = json.Insert(insertPos, bindingTemplate + "\n                ");
        }
        
        return json;
    }
    
    private static string UpdateAttackBinding(string json)
    {
        int actionPos = json.IndexOf("\"action\": \"Attack\"");
        if (actionPos < 0) return json;
        
        int pathStart = json.LastIndexOf("\"path\":", actionPos, actionPos);
        if (pathStart < 0) return json;
        
        int pathValueStart = json.IndexOf(": \"", pathStart) + 3;
        int pathValueEnd = json.IndexOf("\"", pathValueStart);
        
        string currentPath = json.Substring(pathValueStart, pathValueEnd - pathValueStart);
        
        if (currentPath != "<Keyboard>/f")
        {
            json = json.Remove(pathValueStart, pathValueEnd - pathValueStart);
            json = json.Insert(pathValueStart, "<Keyboard>/f");
            Debug.Log("Attack actualizado de " + currentPath + " a F");
        }
        
        return json;
    }
}
#endif
