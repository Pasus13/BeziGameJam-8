using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemDebugger : MonoBehaviour
{
    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        
        if (keyboard == null)
        {
            Debug.LogError("KEYBOARD IS NULL!");
            return;
        }

        if (keyboard.leftArrowKey.wasPressedThisFrame)
            Debug.Log("LEFT ARROW PRESSED!");
        
        if (keyboard.rightArrowKey.wasPressedThisFrame)
            Debug.Log("RIGHT ARROW PRESSED!");
        
        if (keyboard.spaceKey.wasPressedThisFrame)
            Debug.Log("SPACE PRESSED!");
        
        if (keyboard.fKey.wasPressedThisFrame)
            Debug.Log("F KEY PRESSED!");

        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"Input System Check - Left: {keyboard.leftArrowKey.isPressed}, Right: {keyboard.rightArrowKey.isPressed}, Space: {keyboard.spaceKey.isPressed}, F: {keyboard.fKey.isPressed}");
        }
    }
}
