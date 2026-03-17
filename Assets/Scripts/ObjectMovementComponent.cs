
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ObjectMovementComponent : MonoBehaviour
{
    public Vector2 Velocity { get; private set; }
    private const int MaxCollideBounces = 5;
    public float SkinWidth = 0.015f;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;

    private PlayerController _childPlayer;

    public event Action<PlayerController> OnPlayerSqueezed = delegate { };
    public event Action OnObstacleHit = delegate { };
    public event Action OnTerrainHit = delegate { };

    private ContactFilter2D PlayerFilter = new()
    {
        useLayerMask = true,
        layerMask = LayerReference.PlayerLayer,
    };

    private ContactFilter2D TerrainFilter = new()
    {
        useLayerMask = true,
        layerMask = LayerReference.TerrainLayer,
    };

    private ContactFilter2D TerrainAndBoulderFilter = new()
    {
        useLayerMask = true,
        layerMask = LayerReference.TerrainAndBoulder,
    };
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.bodyType = RigidbodyType2D.Kinematic;
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

    public void SetPlayerChild(PlayerController child)
    {
        _childPlayer = child;
    }

    public void UnsetPlayerChild()
    {
        if (_childPlayer != null)
        {
            _childPlayer.MovementController.ExternalVelocity = Vector2.zero;
            _childPlayer = null;
        }
    }

    void FixedUpdate()
    {

        // check for collisions by velocity
        var correctedVelocity = CollideAndSlideVel(_collider.bounds.center, _collider.bounds, Velocity * Time.fixedDeltaTime, TerrainFilter);

        if (correctedVelocity.magnitude + 1e-6 < Velocity.magnitude * Time.fixedDeltaTime)
        {
            OnObstacleHit();
        }


        List<RaycastHit2D> collisionsWithPlayer = new();

        var playerCorrectedVelocity = CollideAndSlideVel(_collider.bounds.center, _collider.bounds, Velocity * Time.fixedDeltaTime, PlayerFilter, collisionsWithPlayer);
        CollideAndSlideVel(_collider.bounds.center, _collider.bounds, Vector2.up * Time.fixedDeltaTime, PlayerFilter, collisionsWithPlayer);
        if (collisionsWithPlayer.Any(c => c))
        {
            _childPlayer = collisionsWithPlayer.First(c => c).collider.GetComponent<PlayerController>();
            _childPlayer.MovementController.ExternalVelocity = correctedVelocity / Time.fixedDeltaTime;
        }
        else if (_childPlayer != null)
        {
            UnsetPlayerChild();
        }

        if (_childPlayer)
        {
            var playerCollisions = new List<RaycastHit2D>();
            var playerColliderBounds = _childPlayer.CollisionController.MainCollider.bounds;
            CollideAndSlideVel(playerColliderBounds.center, playerColliderBounds, Velocity * Time.fixedDeltaTime, TerrainAndBoulderFilter, playerCollisions);
            if (playerCollisions.Any(c => c))
            {
                if (_childPlayer.MovementController.CanBeSqueezed)
                {
                    OnPlayerSqueezed(_childPlayer);
                }
                else
                {
                    OnObstacleHit();
                }
                UnsetPlayerChild();
                return;
            }
        }

        transform.position = transform.position + (Vector3)correctedVelocity;
    }


    public void AddVelocity(Vector2 added)
    {
        Velocity += added;
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

    public void ForceOffset(Vector2 offset)
    {
        Debug.DrawRay(transform.position, offset, Color.purple, 1);
        ForcePosition((Vector2)transform.position + offset);
    }

    public void ForcePosition(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

}