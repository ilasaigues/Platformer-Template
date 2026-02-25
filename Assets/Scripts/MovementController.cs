using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CollisionController))]

public class MovementController : MonoBehaviour
{
    public Vector2 Velocity { get; private set; }

    public bool Grounded = false;

    public DateTime TimeLeftGround;

    private Rigidbody2D _rb;
    private CollisionController _collisonController;
    void Awake()
    {
        _rb = gameObject.GetOrAddComponent<Rigidbody2D>();
        _collisonController = gameObject.GetOrAddComponent<CollisionController>();
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void FixedUpdate()
    {
        var horizontalCorrection = _collisonController.CollideAndSlideVel(
            _rb.position,
            Time.fixedDeltaTime * Velocity.x * Vector2.right,
            LayerReference.TerrainLayer
        );
        var correctedVelocity = horizontalCorrection
         + _collisonController.CollideAndSlideVel(
            _rb.position + horizontalCorrection,
            Time.fixedDeltaTime * Velocity.y * Vector2.up,
            LayerReference.TerrainLayer
        );
        if (Grounded)
        {
            if (correctedVelocity.y != 0)
            {
                TimeLeftGround = DateTime.Now;
                Grounded = false;
            }
        }
        else
        {
            if (Velocity.y < 0 && correctedVelocity.y == 0)
            {
                Grounded = true;
            }
        }
        Velocity = correctedVelocity / Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + correctedVelocity);
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
        ForcePosition(_rb.position + offset);
    }

    public void ForcePosition(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

}
