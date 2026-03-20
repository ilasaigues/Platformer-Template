
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ObjectMovementComponent : MonoBehaviour
{
    public Vector2 Velocity { get; private set; }

    public int Priority => 0;

    private const int MaxCollideBounces = 5;
    public float SkinWidth = 0.015f;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;

    public event Action OnObstacleHit = delegate { };

    private ContactFilter2D PlayerAndTerrainFilter = new()
    {
        useLayerMask = true,
        layerMask = LayerReference.PlayerLayer | LayerReference.TerrainLayer,
    };

    private ContactFilter2D TerrainFilter = new()
    {
        useLayerMask = true,
        layerMask = LayerReference.TerrainLayer,
    };

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        //_rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _collider = GetComponent<BoxCollider2D>();
    }

    public Vector2 CollideAndSlideVel(Vector2 position, Bounds bounds, Vector2 vel, ContactFilter2D filter, List<RaycastHit2D> outHits = null, int recursionDepth = 0)
    {
        Debug.DrawRay(position, vel, Color.red, 1);
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

    void FixedUpdate()
    {
        // check for collisions by velocity
        List<RaycastHit2D> hits = new();

        var correctedVelocity = CollideAndSlideVel(_collider.bounds.center, _collider.bounds, Velocity * Time.fixedDeltaTime, PlayerAndTerrainFilter, hits);

        var player = hits.Where(c => c).Select(c => c.collider.GetComponent<PlayerController>()).FirstOrDefault();

        if (player) // if we collide with player
        {
            if (!(player.PlatformAttacher.IsBeingSqueezed(Velocity * Time.fixedDeltaTime) && player.PlatformAttacher.UnSqueezable))
            {
                correctedVelocity = CollideAndSlideVel(_collider.bounds.center, _collider.bounds, Velocity * Time.fixedDeltaTime, TerrainFilter, hits);
            }
        }

        if (correctedVelocity.magnitude + 1e-6 < Velocity.magnitude * Time.fixedDeltaTime)
        {
            OnObstacleHit();
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        Velocity = correctedVelocity / Time.fixedDeltaTime;

        _rb.linearVelocity = Velocity; //transform.position = transform.position + (Vector3)correctedVelocity;
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        SetVelocity(newVelocity.x, newVelocity.y);
    }

    public void SetVelocity(float? x, float? y)
    {
        x ??= Velocity.x;
        y ??= Velocity.y;
        Velocity = new(x.Value, y.Value);
    }


}