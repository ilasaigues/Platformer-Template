using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputButtonWrapper
{
    public bool Pressed;
    public bool JustPressed;
    public float TimeLastPressed;
    public float TimeSinceLastPressed => Time.time - TimeLastPressed;

    public event Action OnPress = delegate { };
    public event Action OnRelease = delegate { };
    public void OnInputEvent(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Disabled:
                break;
            case InputActionPhase.Waiting:
                break;
            case InputActionPhase.Started:
                JustPressed = true;
                TimeLastPressed = Time.time;
                OnPress();
                break;
            case InputActionPhase.Performed:
                Pressed = true;
                break;
            case InputActionPhase.Canceled:
                Pressed = false;
                OnRelease();
                break;
        }
    }
    public void Update()
    {

    }
    public void LateUpdate()
    {
        JustPressed = false;
    }
}

public class InputAxisWrapper
{
    public Vector2 LastValue;
    public event Action<Vector2> OnValueChanged = delegate { };
    public void OnInputEvent(InputAction.CallbackContext context)
    {
        LastValue = context.ReadValue<Vector2>();
        OnValueChanged(LastValue);
    }
}
