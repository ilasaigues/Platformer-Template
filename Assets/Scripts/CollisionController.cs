using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ColliderController))]
public class CollisionController : MonoBehaviour
{
    private ColliderController _colliderController;

    private Bounds _bounds => _colliderController.ColliderBounds;

    public Vector2 TopLeft => new(_bounds.min.x, _bounds.max.y);
    public Vector2 TopRight => _bounds.max;
    public Vector2 BottomLeft => _bounds.min;
    public Vector2 BottomRight => new(_bounds.max.x, _bounds.min.y);

    private Vector2[] _leftCorners => new Vector2[] { TopLeft, BottomLeft };




    void Awake()
    {
        _colliderController = gameObject.GetOrAddComponent<ColliderController>();
    }

    public RaycastHit2D CheckCollision(Vector2 from, Vector2 direction, float distance, LayerMask layerMask)
    {
        return Physics2D.Raycast(from, direction.normalized, distance, layerMask);
    }

    public bool CheckGround(float distance, LayerMask groundLayerMask)
    {
        return CheckCollision(BottomRight, Vector2.down, distance, groundLayerMask).collider != null ||
                CheckCollision(BottomLeft, Vector2.down, distance, groundLayerMask).collider != null;
    }

    public List<Vector2> GetDirectedCollision(Vector2[] froms, Vector2 direction, float distance, LayerMask layerMask)
    {
        List<Vector2> collidingOriginPoints = new();

        foreach (var from in froms)
        {
            if (CheckCollision(from, direction, distance, layerMask).collider != null)
            {
                collidingOriginPoints.Add(from);
            }
        }
        return collidingOriginPoints;
    }

    public List<Vector2> GetVerticalCollisions(Vector2 direction, float distance, LayerMask layerMask)
    {
        Vector2[] origins = direction.y > 0 ? new Vector2[] { TopLeft, TopRight } : new Vector2[] { BottomLeft, BottomRight };

        return GetDirectedCollision(origins, Vector2.up * direction.y, distance, layerMask);
    }

    public List<Vector2> GetHorizontalCollisions(Vector2 direction, float distance, LayerMask layerMask)
    {
        Vector2[] origins = direction.x > 0 ? new Vector2[] { TopRight, BottomRight } : new Vector2[] { TopLeft, BottomLeft };

        return GetDirectedCollision(origins, Vector2.right * direction.x, distance, layerMask);
    }

    public float? GetClosestCollisonSurfaceDistance(Vector2 direction, Bounds colliderBounds, float checkDistance, LayerMask layerMask)
    {

        var hits = Physics2D.BoxCastAll(colliderBounds.center, colliderBounds.size, 0, direction, checkDistance, layerMask)
            .Where(hit => hit.fraction != 0);


        float correction;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) // horizontal
        {
            correction = colliderBounds.extents.x;
        }
        else // vertical
        {
            correction = colliderBounds.extents.y;
        }

        // add contact filter
        float? result = null;
        foreach (var hit in hits)
        {
            result = result == null ? hit.distance : Mathf.Min(result.Value, hit.distance);
        }
        return result == null ? result : result - correction;
    }

}
