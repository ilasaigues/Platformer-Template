using Unity.Cinemachine;
using UnityEngine;


public interface IInputOverride { }

public class ButtonOverride : IInputOverride
{
    public bool Pressed;
    public int DurationMilliseconds;
    public InputButtonWrapper Button;
    public ButtonOverride(InputButtonWrapper button, bool pressed, int durationMilliseconds)
    {
        Button = button;
        Pressed = pressed;
        DurationMilliseconds = durationMilliseconds;
    }
}
public class AxisOverride : IInputOverride
{
    public Vector2 Direction;
    public int DurationMilliseconds;
    public InputAxisWrapper Axis;
    public AxisOverride(InputAxisWrapper axis, Vector2 direction, int durationMilliseconds)
    {
        Axis = axis;
        Direction = direction;
        DurationMilliseconds = durationMilliseconds;
    }
}

