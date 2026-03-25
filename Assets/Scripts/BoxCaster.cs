using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BoxCaster2D
{

    public const float SkinWidth = 0.015f;
    public const int MaxCollideBounces = 10;

    public static List<RaycastHit2D> GetHits(Vector2 position, Bounds bounds, Vector2 vel, ContactFilter2D filter)
    {
        float dist = vel.magnitude + SkinWidth;
        var hits = new List<RaycastHit2D>();
        Physics2D.BoxCast(position, bounds.size, 0, vel.normalized, filter, hits, dist);
        return hits;
    }

    public static Vector2 CollideAndSlideVel(Vector2 position, Bounds bounds, Vector2 vel, ContactFilter2D filter, List<RaycastHit2D> outHits = null, int recursionDepth = 0)
    {
        // shrink once, don't shrink Z
        if (recursionDepth == 0)
        {
            bounds.Expand(-2 * SkinWidth * Vector2.one);
        }

        if (recursionDepth >= MaxCollideBounces) return Vector2.zero;
        float dist = vel.magnitude + SkinWidth;

        List<RaycastHit2D> hits = GetHits(position, bounds, vel, filter);

        if (hits.Any(hit => hit))
        {
            if (vel.magnitude < 1e-6) return Vector2.zero;
            outHits?.AddRange(hits.Where(hit => hit));
            var closestHit = hits.First(hit => hit);
            Vector2 snapToSurface = vel.normalized * (closestHit.distance - SkinWidth);
            Vector2 leftover = vel - snapToSurface;
            if (snapToSurface.magnitude <= SkinWidth) snapToSurface = Vector2.zero;
            float mag = leftover.magnitude;
            leftover = Vector3.ProjectOnPlane(leftover, closestHit.normal).normalized;
            leftover *= mag;
            return snapToSurface + CollideAndSlideVel(position + snapToSurface, bounds, leftover, filter, outHits, recursionDepth + 1);
        }

        return vel;
    }
}