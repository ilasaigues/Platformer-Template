using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Input Handler", menuName = "Scriptable Objects/InputHandler")]
public class InputHandler : ScriptableObject, GameInputs.IPlayerActions
{
    private GameInputs _gameInput;

    public bool IsJumpPressed;

    void OnEnable()
    {
        if (_gameInput == null) _gameInput = new GameInputs();

        _gameInput.Player.SetCallbacks(this);
        _gameInput.Player.Enable();
    }
    void OnDisable()
    {
        _gameInput.Player.Disable();
    }

    public event Action JumpPressed = delegate { };
    public event Action JumpReleased = delegate { };
    public event Action<Vector2> MoveEvent = delegate { };

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            JumpPressed.Invoke();
            IsJumpPressed = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            JumpReleased.Invoke();
            IsJumpPressed = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }
}
