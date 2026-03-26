using UnityEngine;


public static class DebugExtensions
{
    public static void DrawBox(Vector2 position, float size, Color color)
    {
       Vector2 offset = Vector2.one * size/2;
       Vector2 posA = position + new Vector2(-offset.x, offset.y);
       Vector2 posB = position + offset;
       Vector2 posC = position - offset;
       Vector2 posD = position + new Vector2 (offset.x, -offset.y);

       Debug.DrawLine(posA,  posA + Vector2.right * size, color, 0.1f);
       Debug.DrawLine(posB, posB + Vector2.down * size, color, 0.1f);
       Debug.DrawLine(posC, posC + Vector2.up * size, color, 0.1f);
       Debug.DrawLine(posD, posD -Vector2.right * size, color, 0.1f);
    }
}

