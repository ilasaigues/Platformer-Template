public static class MathfExtensions
{
    public static int Sign0(this float value)
    {
        return value == 0 ? 0 : (value > 0 ? 1 : -1);
    }
}
