using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BoxCaster2D
{

    public const float SkinWidth = 0.015f;
    public const int MaxCollideBounces = 10;

    public static List<RaycastHit2D> GetHits(Vector2 position, Bounds bounds, Vector2 vel, LayerMask layer, float angleArc = 180)
    {
        bounds.Expand(-2 * SkinWidth * Vector2.one);
        float dist = vel.magnitude + SkinWidth;
        var hits = new List<RaycastHit2D>();
        var signedAngle = Vector2.SignedAngle(Vector2.right, -vel.normalized);
        var filter = layer.ToContactFilter2D();
        Physics2D.BoxCast(position, bounds.size, 0, vel.normalized, filter, hits, dist);
        bounds.Expand(2 * SkinWidth * Vector2.one);
        if (vel.magnitude != 0)
        {
            hits = hits.Where(hit => Vector2.Angle(hit.normal, vel.normalized) >= 180 - angleArc).ToList();
        }

        return hits;
    }

    public static Vector2 CollideAndSlideVel(Vector2 position, Bounds bounds, Vector2 vel, LayerMask layer, List<RaycastHit2D> outHits = null, int recursionDepth = 0)
    {

        if (recursionDepth >= MaxCollideBounces) return Vector2.zero;
        float dist = vel.magnitude + SkinWidth;

        List<RaycastHit2D> hits = GetHits(position, bounds, vel, layer, 1);

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
            return snapToSurface + CollideAndSlideVel(position + snapToSurface, bounds, leftover, layer, outHits, recursionDepth + 1);
        }

        return vel;
    }
}