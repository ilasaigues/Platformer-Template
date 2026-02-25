using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollisionController : MonoBehaviour
{
    private Collider2D _collider;



    private const int MaxCollideBounces = 5;
    private const float SkinWidth = 0.015f;

    void Awake()
    {
        _collider = gameObject.GetOrAddComponent<BoxCollider2D>();
    }

    public Vector2 CollideAndSlideVel(Vector2 position, Vector2 vel, LayerMask collisionLayer, int recursionDepth = 0)
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(-2 * SkinWidth);
        if (recursionDepth >= MaxCollideBounces) return Vector2.zero;
        float dist = vel.magnitude + SkinWidth;

        RaycastHit2D hit = Physics2D.BoxCast(position, bounds.size, 0, vel.normalized, dist, collisionLayer);

        if (hit)
        {
            Vector2 snapToSurface = vel.normalized * (hit.distance - SkinWidth);
            Vector2 leftover = vel - snapToSurface;
            if (snapToSurface.magnitude <= SkinWidth) snapToSurface = Vector2.zero;
            float mag = leftover.magnitude;
            leftover = Vector3.ProjectOnPlane(leftover, hit.normal);//.normalized;
            //leftover *= mag;
            return snapToSurface + CollideAndSlideVel(position + snapToSurface, leftover, collisionLayer, recursionDepth + 1);
        }

        return vel;
    }

    public bool CheckCollision(Vector2 position, Vector2 vel, LayerMask collisionLayer)
    {
        return Physics2D.BoxCast(position, _collider.bounds.size, 0, vel.normalized, vel.magnitude, collisionLayer);
    }

}
