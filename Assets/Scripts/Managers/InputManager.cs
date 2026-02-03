using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;
    public static bool DashIsPressed;
    public static bool ConfirmWasPressed;
    public static bool MagicWasPressed;
    public static bool Magic1WasPressed;
    public static bool Magic2WasPressed;
    public static bool Magic3WasPressed;
    public static bool QTEButtonLeftWasPressed;
    public static bool QTEButtonUpWasPressed;
    public static bool QTEButtonDownWasPressed;
    public static bool QTEButtonRightWasPressed;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _dashAction;
    private InputAction _confirmAction;
    private InputAction _magic1Action;
    private InputAction _magic2Action;
    private InputAction _magic3Action;
    private InputAction _qteButtonLeftAction;
    private InputAction _qteButtonUpAction;
    private InputAction _qteButtonDownAction;
    private InputAction _qteButtonRightAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        if (PlayerInput == null)
        {
            Debug.LogWarning("InputManager: No PlayerInput component found. Disabling InputManager.");
            enabled = false;
            return;
        }
        else
        {
            _moveAction = PlayerInput.actions["Move"];
            _jumpAction = PlayerInput.actions["Jump"];
            _dashAction  = PlayerInput.actions["Dash"];
            _confirmAction = PlayerInput.actions["Enter"];


            _magic1Action = TryGetAction("Magic1");
            _magic2Action = TryGetAction("Magic2");
            _magic3Action = TryGetAction("Magic3");
            
            _qteButtonLeftAction = PlayerInput.actions["QTEButtonLeft"];
            _qteButtonUpAction = PlayerInput.actions["QTEButtonUp"];
            _qteButtonDownAction = PlayerInput.actions["QTEButtonDown"];
            _qteButtonRightAction = PlayerInput.actions["QTEButtonRight"];

            PlayerInput.actions.Enable();
        }
    }
    
    private InputAction TryGetAction(string actionName)
    {
        try
        {
            return PlayerInput.actions[actionName];
        }
        catch
        {
            Debug.LogWarning($"InputManager: Action '{actionName}' not found. Please update Input Actions via Tools → Update Input Actions for Magic System");
            return null;
        }
    }

    private void Update()
    {
            Movement = _moveAction.ReadValue<Vector2>();

            JumpWasPressed = _jumpAction.WasPressedThisFrame();
            JumpIsHeld = _jumpAction.IsPressed();
            JumpWasReleased = _jumpAction.WasReleasedThisFrame();

            DashIsPressed = _dashAction.IsPressed();

            ConfirmWasPressed = _confirmAction.IsPressed();
            
            Magic1WasPressed = _magic1Action != null && _magic1Action.WasPressedThisFrame();
            Magic2WasPressed = _magic2Action != null && _magic2Action.WasPressedThisFrame();
            Magic3WasPressed = _magic3Action != null && _magic3Action.WasPressedThisFrame();
            
            QTEButtonLeftWasPressed = _qteButtonLeftAction.WasPressedThisFrame();
            QTEButtonUpWasPressed = _qteButtonUpAction.WasPressedThisFrame();
            QTEButtonDownWasPressed = _qteButtonDownAction.WasPressedThisFrame();
            QTEButtonRightWasPressed = _qteButtonRightAction.WasPressedThisFrame();
    }
}
