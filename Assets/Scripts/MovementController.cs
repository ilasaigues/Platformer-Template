using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CollisionController))]
[RequireComponent(typeof(PlayerController))]
public class MovementController : MonoBehaviour
{
    public Vector2 Velocity { get; private set; }

    public bool Grounded = false;
    public bool OnOneWayPlatform = false;
    public int LastHorizontalDirection;

    public DateTime TimeLeftGround;

    public bool IgnoreOneWay = false;

    private Rigidbody2D _rb;
    private CollisionController _collisonController;
    private PlayerController _playerController;

    private Bounds _mainColliderBounds => _collisonController.MainCollider.bounds;
    private Bounds _footColliderBounds => _collisonController.FootCollider.bounds;

    private ContactFilter2D OneWayFilter = new()
    {
        useLayerMask = true,
        layerMask = LayerReference.OneWayPlatformLayer,
        useNormalAngle = true,
        maxNormalAngle = 91,
        minNormalAngle = 89,
    };

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


        bool ledgeCorrected = false;

        // Horizontal correction
        var correctedHorizontal = _collisonController.CollideAndSlideVel(transform.position, _mainColliderBounds, originalHorizontal, LayerReference.TerrainLayer);

        if (correctedHorizontal.magnitude < Mathf.Abs(originalHorizontal.x)) // if collided and shrunk vector
        {
            var ledgeCorrection = GetCorrection(transform.position, originalHorizontal, correctedHorizontal, Vector2.up * _playerController.PlayerStats.ledgeCorrectionUp, Vector2.down * _playerController.PlayerStats.ledgeCorrectionDown, LayerReference.TerrainLayer);

            if (ledgeCorrection != Vector2.zero)
            {
                ForceOffset(ledgeCorrection);
                ledgeCorrected = true;
                correctedHorizontal = originalHorizontal;
            }
        }


        // Vertical correction
        var correctedVertical = _collisonController.CollideAndSlideVel(transform.position + (Vector3)correctedHorizontal, _mainColliderBounds, originalVertical, LayerReference.TerrainLayer);

        if (!ledgeCorrected && !Grounded && originalVertical.y > 0 && correctedVertical.magnitude < Mathf.Abs(originalVertical.y)) // if collided and shrunk vector
        {
            var ceilingCorrection = GetCorrection(transform.position + (Vector3)correctedHorizontal, originalVertical, correctedVertical, Vector2.left * _playerController.PlayerStats.ceilingCorrection, Vector2.right * _playerController.PlayerStats.ceilingCorrection, LayerReference.TerrainLayer);

            if (ceilingCorrection != Vector2.zero && (ceilingCorrection.x.Sign0() * originalHorizontal.x.Sign0()) >= 0)
            {
                ForceOffset(ceilingCorrection);
                ledgeCorrected = true;
                correctedVertical = originalVertical;
            }
        }


        // One Way Correction
        var hitList = new List<RaycastHit2D>();
        var oneWayCorrection = _collisonController.CollideAndSlideVel((Vector2)_footColliderBounds.center + correctedHorizontal, _footColliderBounds, originalVertical, OneWayFilter, hitList);

        if (oneWayCorrection != originalVertical) // collided against one-way platform
        {
            if (!IgnoreOneWay)
            {
                OnOneWayPlatform = true;
                //if (correctedVertical.y > 0) oneWayCorrection += Vector2.up * _footColliderBounds.size.y;
                if (hitList.Any(hit => hit.distance * hit.fraction == 0))
                {
                    ForceOffset(Vector2.up * _footColliderBounds.size.y);
                }
                correctedVertical = oneWayCorrection;
            }
        }
        else
        {
            IgnoreOneWay = false;
            OnOneWayPlatform = false;
        }



        var correctedVelocity = correctedHorizontal + correctedVertical;
        if (Grounded)
        {
            if (!Mathf.Approximately(correctedVelocity.y, 0))
            {
                TimeLeftGround = DateTime.Now;
                Grounded = false;
            }
        }
        else
        {
            if (Velocity.y < 0 && Mathf.Approximately(correctedVelocity.y, 0))
            {
                Grounded = true;
            }
        }


        if (correctedVelocity.x != 0)
        {
            LastHorizontalDirection = correctedVelocity.x.Sign0();
        }

        Velocity = correctedVelocity / Time.fixedDeltaTime;

        Debug.DrawRay((Vector2)transform.position, correctedVelocity / 2, Color.green, 1);
        Debug.DrawRay((Vector2)transform.position + correctedVelocity / 2, correctedVelocity / 2, Color.yellow, 1);

        // Apply movement
        transform.position = transform.position + (Vector3)correctedVelocity;
    }

    Vector2 GetCorrection(Vector2 position, Vector2 originalDirection, Vector2 correctedDirection, Vector2 offsetA, Vector2 offsetB, LayerMask layerMask)
    {
        Vector2 positionA = position + offsetA;
        Vector2 positionB = position + offsetB;

        var collisionA = _collisonController.CollideAndSlideVel(positionA, _mainColliderBounds, originalDirection, layerMask);
        var collisionB = _collisonController.CollideAndSlideVel(positionB, _mainColliderBounds, originalDirection, layerMask);
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
        Debug.DrawRay(transform.position, offset, Color.purple, 1);
        ForcePosition((Vector2)transform.position + offset);
    }

    public void ForcePosition(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

}
