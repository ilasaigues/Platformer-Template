
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ObjectMovementComponent : MonoBehaviour
{
    public Vector2 Velocity { get; private set; }

    private Rigidbody2D _rb;
    private BoxCollider2D _collider;

    //private PlayerController _childPlayer;

    public Action<PlayerController> OnPlayerSqueezed = delegate { };
    public event Action OnObstacleHit = delegate { };
    public event Action OnTerrainHit = delegate { };


    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _collider = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        // check for collisions by velocity
        var mainBounds = _collider.bounds;
        var correctedVelocity = BoxCaster2D.CollideAndSlideVel(mainBounds.center, mainBounds, Velocity * Time.fixedDeltaTime, LayerReference.TerrainLayer.ToContactFilter2D());

        if (correctedVelocity.magnitude + 1e-6 < Velocity.magnitude * Time.fixedDeltaTime)
        {
            OnObstacleHit();
        }

        List<RaycastHit2D> collisionsWithPlayer = BoxCaster2D.GetHits(mainBounds.center, mainBounds, Velocity * Time.fixedDeltaTime, LayerReference.PlayerLayer.ToContactFilter2D());
        collisionsWithPlayer.AddRange(BoxCaster2D.GetHits(mainBounds.center, mainBounds, Vector2.up * Time.fixedDeltaTime, LayerReference.PlayerLayer.ToContactFilter2D()));

        if (collisionsWithPlayer.Any(c => c))
        {
            var playerMovementController = collisionsWithPlayer.First(c => c).collider.GetComponent<MovementController>();
            playerMovementController.SetParentObject(this);
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