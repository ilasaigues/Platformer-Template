using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputButtonWrapper
{
    public bool Pressed;
    public bool JustPressed;
    public float TimeLastPressed = float.NegativeInfinity;
    private TimeContext _timeContext;

    public float TimeSinceLastPressed => _timeContext.Time - TimeLastPressed;



    public event Action OnPress = delegate { };
    public event Action OnRelease = delegate { };

    public InputButtonWrapper(TimeContext timeContext)
    {
        _timeContext = timeContext;
    }

    public void OnInputEvent(InputActionPhase phase)
    {
        switch (phase)
        {
            case InputActionPhase.Disabled:
                break;
            case InputActionPhase.Waiting:
                break;
            case InputActionPhase.Started:
                JustPressed = true;
                TimeLastPressed = _timeContext.Time;
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
    public void OnInputEvent(Vector2 axisValue)
    {
        LastValue = axisValue;
        OnValueChanged(LastValue);
    }
}
