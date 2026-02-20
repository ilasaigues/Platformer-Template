using UnityEngine;

public static class MathfExtensions
{
    public static float Sign0(this float value)
    {
        return value == 0 ? 0 : (value > 0 ? 1 : -1);
    }
}
