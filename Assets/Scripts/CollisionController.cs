using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollisionController : MonoBehaviour
{


    private const int MaxCollideBounces = 5;
    public float SkinWidth = 0.015f;

    public BoxCollider2D MainCollider;
    public BoxCollider2D FootCollider;

    private const float FootColliderHeight = 0.125f;

    void Start()
    {
        ResizeFloorCollider();
    }

    public void ResizeMainCollider(Vector2 size, Vector2 offset)
    {
        MainCollider.offset = offset;
        MainCollider.size = size;
        ResizeFloorCollider();
    }



    private void ResizeFloorCollider()
    {
        FootCollider.offset = Vector2.up * (MainCollider.offset.y + (-MainCollider.size.y + FootColliderHeight) * 0.5f);
        FootCollider.size = new Vector2(MainCollider.size.x, FootColliderHeight);
    }


    public Vector2 CollideAndSlideVel(Vector2 position, Bounds bounds, Vector2 vel, LayerMask collisionLayer, List<RaycastHit2D> outHits = null)
    {
        var filter = new ContactFilter2D()
        {
            layerMask = collisionLayer,
            useLayerMask = true,
        };
        return CollideAndSlideVel(position, bounds, vel, filter, outHits);
    }

    public Vector2 CollideAndSlideVel(Vector2 position, Bounds bounds, Vector2 vel, ContactFilter2D filter, List<RaycastHit2D> outHits = null, int recursionDepth = 0)
    {
        // shrink once, don't shrink Z
        if (recursionDepth == 0)
        {
            bounds.Expand(-2 * SkinWidth * Vector2.one);
        }

        if (recursionDepth >= MaxCollideBounces) return Vector2.zero;
        float dist = vel.magnitude + SkinWidth;

        RaycastHit2D[] hits = new RaycastHit2D[5];
        int hitCount = Physics2D.BoxCast(position, bounds.size, 0, vel.normalized, filter, hits, dist);

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
