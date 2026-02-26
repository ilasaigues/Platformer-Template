using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CollisionController))]
[RequireComponent(typeof(PlayerController))]
public class MovementController : MonoBehaviour
{
    public Vector2 Velocity { get; private set; }

    public bool Grounded = false;

    public DateTime TimeLeftGround;

    private Rigidbody2D _rb;
    private CollisionController _collisonController;
    private PlayerController _playerController;

    void Awake()
    {
        _rb = gameObject.GetOrAddComponent<Rigidbody2D>();
        _collisonController = gameObject.GetOrAddComponent<CollisionController>();
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _playerController = gameObject.GetOrAddComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        if (Velocity.y < _playerController.PlayerStats.fallVelocityCap)
        {
            SetVelocity(null, _playerController.PlayerStats.fallVelocityCap);
        }

        Vector2 originalVertical = Time.fixedDeltaTime * Velocity.y * Vector2.up;

        Vector2 originalHorizontal = Time.fixedDeltaTime * Velocity.x * Vector2.right;
        var correctedHorizontal = _collisonController.CollideAndSlideVel(transform.position, originalHorizontal, LayerReference.TerrainLayer);
        if (correctedHorizontal.magnitude < Mathf.Abs(originalHorizontal.x)) // if collided and shrunk vector
        {
            var correction = GetCorrection(
                transform.position,
                originalHorizontal,
                correctedHorizontal,
                Vector2.up * _playerController.PlayerStats.ledgeCorrectionUp,
                Vector2.down * _playerController.PlayerStats.ledgeCorrectionDown,
                LayerReference.TerrainLayer
            );

            if (correction != Vector2.zero)
            {
                ForceOffset(correction);
                correctedHorizontal = originalHorizontal;
            }
        }


        var correctedVertical = _collisonController.CollideAndSlideVel(transform.position + (Vector3)correctedHorizontal, originalVertical, LayerReference.TerrainLayer);


        if (originalVertical.y > 0 && correctedVertical.magnitude < Mathf.Abs(originalVertical.y)) // if collided and shrunk vector
        {
            var correction = GetCorrection(
                transform.position,
                originalVertical,
                correctedVertical,
                Vector2.left * _playerController.PlayerStats.ceilingCorrection,
                Vector2.right * _playerController.PlayerStats.ceilingCorrection,
                LayerReference.TerrainLayer
            );

            if (correction != Vector2.zero)
            {
                ForceOffset(correction);
                correctedVertical = originalVertical;
            }
        }


        var correctedVelocity = correctedHorizontal + correctedVertical;
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

        // YELLOW AND GREEN SPEEEEEED LINE
        Debug.DrawRay((Vector2)transform.position, correctedVelocity / 2, Color.green, 1);
        Debug.DrawRay((Vector2)transform.position + correctedVelocity / 2, correctedVelocity / 2, Color.yellow, 1);

        transform.position = transform.position + (Vector3)correctedVelocity;
    }

    Vector2 GetCorrection(Vector2 position, Vector2 originalDirection, Vector2 correctedDirection, Vector2 offsetA, Vector2 offsetB, LayerMask layerMask)
    {
        Vector2 positionA = position + offsetA;
        Vector2 positionB = position + offsetB;

        var collisionA = _collisonController.CollideAndSlideVel(positionA, originalDirection, layerMask);
        var collisionB = _collisonController.CollideAndSlideVel(positionB, originalDirection, layerMask);
        if (collisionA.magnitude > correctedDirection.magnitude)
        {
            return offsetA;
        }
        else if (collisionB.magnitude > correctedDirection.magnitude)
        {
            return offsetB;
        }
        return Vector2.zero;
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
        ForcePosition((Vector2)transform.position + offset);
    }

    public void ForcePosition(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

}
