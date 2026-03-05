using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour, GameInputs.IPlayerActions
{
    private GameInputs _gameInput;

    public InputButtonWrapper JumpButton;
    public InputButtonWrapper RockButton;
    public InputButtonWrapper DashButton;

    List<InputButtonWrapper> _buttons = new();
    public InputAxisWrapper MoveAxis;

    void Awake()
    {
        MoveAxis = new InputAxisWrapper();
        JumpButton = GetNewButton();
        RockButton = GetNewButton();
        DashButton = GetNewButton();
    }


    InputButtonWrapper GetNewButton()
    {
        var wrapper = new InputButtonWrapper();
        _buttons.Add(wrapper);
        return wrapper;
    }

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

    public event Action<Vector2> MoveEvent = delegate { };

    public void OnJump(InputAction.CallbackContext context)
    {
        JumpButton.OnInputEvent(context);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveAxis.OnInputEvent(context);
    }

    void Update()
    {
        _buttons.ForEach(button => button.Update());
    }

    void LateUpdate()
    {
        _buttons.ForEach(button => button.LateUpdate());
    }

    public void OnRock(InputAction.CallbackContext context)
    {
        RockButton.OnInputEvent(context);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        DashButton.OnInputEvent(context);
    }
}
