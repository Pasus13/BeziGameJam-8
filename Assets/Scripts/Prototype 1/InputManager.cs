using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;
    public static bool RunIsHeld;
    public static bool AttackWasPressed;
    public static bool MagicWasPressed;
    public static bool QTEButton1WasPressed;
    public static bool QTEButton2WasPressed;
    public static bool QTEButton3WasPressed;
    public static bool QTEButton4WasPressed;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _attackAction;
    private InputAction _magicAction;
    private InputAction _qteButton1Action;
    private InputAction _qteButton2Action;
    private InputAction _qteButton3Action;
    private InputAction _qteButton4Action;

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
            _runAction  = PlayerInput.actions["Run"];
            _attackAction = PlayerInput.actions["Attack"];
            _magicAction = PlayerInput.actions["Magic"];
            _qteButton1Action = PlayerInput.actions["QTEButton1"];
            _qteButton2Action = PlayerInput.actions["QTEButton2"];
            _qteButton3Action = PlayerInput.actions["QTEButton3"];
            _qteButton4Action = PlayerInput.actions["QTEButton4"];
        }
    }

    private void Update()
    {
            Movement = _moveAction.ReadValue<Vector2>();

            JumpWasPressed = _jumpAction.WasPressedThisFrame();
            JumpIsHeld = _jumpAction.IsPressed();
            JumpWasReleased = _jumpAction.WasReleasedThisFrame();

            RunIsHeld = _runAction.IsPressed();
            
            AttackWasPressed = _attackAction.WasPressedThisFrame();
            MagicWasPressed = _magicAction.WasPressedThisFrame();
            
            QTEButton1WasPressed = _qteButton1Action.WasPressedThisFrame();
            QTEButton2WasPressed = _qteButton2Action.WasPressedThisFrame();
            QTEButton3WasPressed = _qteButton3Action.WasPressedThisFrame();
            QTEButton4WasPressed = _qteButton4Action.WasPressedThisFrame();
    }
}
