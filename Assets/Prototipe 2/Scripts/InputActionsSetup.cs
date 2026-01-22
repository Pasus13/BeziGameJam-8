using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public static class InputActionsSetup
{
    static InputActionsSetup()
    {
        EditorApplication.delayCall += SetupInputActions;
    }

    private static void SetupInputActions()
    {
        string assetPath = "Assets/Inputs/Prototype 1/PlayerInputActions.inputactions";
        InputActionAsset inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(assetPath);
        
        if (inputActions == null)
        {
            Debug.LogWarning($"InputActions asset not found at {assetPath}");
            return;
        }

        InputActionMap playerMap = inputActions.FindActionMap("Player");
        if (playerMap == null)
        {
            Debug.LogWarning("Player action map not found");
            return;
        }

        bool needsSave = false;

        if (playerMap.FindAction("Magic") == null)
        {
            playerMap.AddAction("Magic", InputActionType.Button);
            var magicAction = playerMap.FindAction("Magic");
            magicAction.AddBinding("<Keyboard>/a");
            needsSave = true;
            Debug.Log("Added Magic action");
        }

        if (playerMap.FindAction("QTEButton1") == null)
        {
            playerMap.AddAction("QTEButton1", InputActionType.Button);
            var qteButton1Action = playerMap.FindAction("QTEButton1");
            qteButton1Action.AddBinding("<Keyboard>/q");
            needsSave = true;
            Debug.Log("Added QTEButton1 action");
        }

        if (playerMap.FindAction("QTEButton2") == null)
        {
            playerMap.AddAction("QTEButton2", InputActionType.Button);
            var qteButton2Action = playerMap.FindAction("QTEButton2");
            qteButton2Action.AddBinding("<Keyboard>/w");
            needsSave = true;
            Debug.Log("Added QTEButton2 action");
        }

        if (playerMap.FindAction("QTEButton3") == null)
        {
            playerMap.AddAction("QTEButton3", InputActionType.Button);
            var qteButton3Action = playerMap.FindAction("QTEButton3");
            qteButton3Action.AddBinding("<Keyboard>/e");
            needsSave = true;
            Debug.Log("Added QTEButton3 action");
        }

        if (playerMap.FindAction("QTEButton4") == null)
        {
            playerMap.AddAction("QTEButton4", InputActionType.Button);
            var qteButton4Action = playerMap.FindAction("QTEButton4");
            qteButton4Action.AddBinding("<Keyboard>/r");
            needsSave = true;
            Debug.Log("Added QTEButton4 action");
        }

        if (needsSave)
        {
            EditorUtility.SetDirty(inputActions);
            AssetDatabase.SaveAssets();
            Debug.Log("Input Actions updated successfully!");
        }
    }
}
#endif
