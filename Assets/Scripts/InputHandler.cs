using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour, GameInputs.IPlayerActions
{
    private GameInputs _gameInput;

    public InputButtonWrapper JumpButton;
    public InputAxisWrapper MoveAxis;

    void Start()
    {
        MoveAxis = new InputAxisWrapper();
        JumpButton = new InputButtonWrapper();
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
        JumpButton.Update();
    }

    void LateUpdate()
    {
        JumpButton.LateUpdate();
    }

}
