using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollisionController : MonoBehaviour
{


    private const int MaxCollideBounces = 5;
    public float SkinWidth = 0.015f;

    public Collider2D MainCollider;
    public Collider2D FootCollider;


    public Vector2 CollideAndSlideVel(Vector2 position, Bounds bounds, Vector2 vel, LayerMask collisionLayer, int recursionDepth = 0)
    {
        var filter = new ContactFilter2D()
        {
            layerMask = collisionLayer,
            useLayerMask = true,
        };
        return CollideAndSlideVel(position, bounds, vel, filter, recursionDepth);
    }

    public Vector2 CollideAndSlideVel(Vector2 position, Bounds bounds, Vector2 vel, ContactFilter2D filter, int recursionDepth = 0)
    {
        // shrink once, don't shrink Z
        if (recursionDepth == 0)
        {
            bounds.Expand(-2 * SkinWidth * Vector2.one);
        }

        if (recursionDepth >= MaxCollideBounces) return Vector2.zero;
        if (vel.magnitude < 1e-6) return Vector2.zero;
        float dist = vel.magnitude + SkinWidth;

        RaycastHit2D[] hits = new RaycastHit2D[5];
        int hitCount = Physics2D.BoxCast(position, bounds.size, 0, vel.normalized, filter, hits, dist);

        Vector2 overlapCorrection = Vector2.zero;
        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];
            if (hit)
            {
                if (hit.distance * hit.fraction == 0) // hit is result of overlapping origin of boxcast and collider
                {

                    Debug.DrawLine(position, (position + hit.point) / 2, Color.green);
                    Debug.DrawLine((position + hit.point) / 2, hit.point, Color.cyan);
                    var hitVector = hit.point - position; // the lenth of this is half of how much the bound peeks out of the collider

                    Debug.Log($"<color=red>{hitVector.y} , {bounds.size.y / 2}</color>");


                    float correctionDistance = 0;
                    if (Mathf.Abs(vel.y) > Mathf.Abs(vel.x)) // moving vertically
                    {
                        correctionDistance = bounds.size.y - 2 * Mathf.Abs(hitVector.y);
                    }
                    else // moving horizontally
                    {
                        correctionDistance = bounds.size.x - 2 * Mathf.Abs(hitVector.x);
                    }
                    overlapCorrection = -hitVector.normalized * correctionDistance;
                    Debug.DrawLine(position, hit.point, Color.red, .1f);
                }
            }
        }


        if (hits.Any(hit => hit))
        {
            var closestHit = hits.First(hit => hit);
            Vector2 snapToSurface = vel.normalized * (closestHit.distance - SkinWidth);
            Vector2 leftover = vel - snapToSurface;
            if (snapToSurface.magnitude <= SkinWidth) snapToSurface = Vector2.zero;
            float mag = leftover.magnitude;
            leftover = Vector3.ProjectOnPlane(leftover, closestHit.normal);//.normalized;
            //leftover *= mag;
            return snapToSurface + CollideAndSlideVel(position + snapToSurface, bounds, leftover, filter, recursionDepth + 1) + overlapCorrection;
        }

        return vel;
    }


}
