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

    public bool CanBeSqueezed = true;


    public float TimeLeftGround;

    public bool IgnoreOneWay = false;

    private Rigidbody2D _rb;
    private CollisionController _collisonController;
    private PlayerController _playerController;

    public Bounds MainColliderBounds => _collisonController.MainCollider.bounds;
    private Bounds _footColliderBounds => _collisonController.FootCollider.bounds;

    public ObjectMovementComponent ParentObject { get; private set; }

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
        CanBeSqueezed = true;
        _rb = gameObject.GetOrAddComponent<Rigidbody2D>();
        _collisonController = gameObject.GetOrAddComponent<CollisionController>();
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _playerController = gameObject.GetOrAddComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        CheckAndFixOverlap();

        Vector2 parentVelocity = GetParentVelocity();
        var combinedBaseVelocity = Velocity + parentVelocity;

        if (combinedBaseVelocity.y < _playerController.PlayerStats.fallVelocityCap)
        {
            SetVelocity(null, _playerController.PlayerStats.fallVelocityCap);
        }
        Vector2 originalVertical = Time.fixedDeltaTime * combinedBaseVelocity * Vector2.up;
        Vector2 originalHorizontal = Time.fixedDeltaTime * combinedBaseVelocity * Vector2.right;


        bool ledgeCorrected = false;

        // Horizontal correction
        var horizontalHits = new List<RaycastHit2D>();
        var correctedHorizontal = _collisonController.CollideAndSlideVel(transform.position, MainColliderBounds, originalHorizontal, LayerReference.TerrainAndBoulder, horizontalHits);

        if (correctedHorizontal.magnitude < Mathf.Abs(originalHorizontal.x)) // if collided and shrunk vector
        {
            var ledgeCorrection = GetCorrection(transform.position, originalHorizontal, correctedHorizontal, Vector2.up * _playerController.PlayerStats.ledgeCorrectionUp, Vector2.down * _playerController.PlayerStats.ledgeCorrectionDown, LayerReference.TerrainAndBoulder);

            if (ledgeCorrection != Vector2.zero)
            {
                ForceOffset(ledgeCorrection);
                ledgeCorrected = true;
                correctedHorizontal = originalHorizontal;
            }
        }


        // Vertical correction
        var verticalHits = new List<RaycastHit2D>();
        var correctedVertical = _collisonController.CollideAndSlideVel(transform.position + (Vector3)correctedHorizontal, MainColliderBounds, originalVertical, LayerReference.TerrainAndBoulder, verticalHits);

        if (!ledgeCorrected && !Grounded && originalVertical.y > 0 && correctedVertical.magnitude < Mathf.Abs(originalVertical.y)) // if collided and shrunk vector
        {
            var ceilingCorrection = GetCorrection(transform.position + (Vector3)correctedHorizontal, originalVertical, correctedVertical, Vector2.left * _playerController.PlayerStats.ceilingCorrection, Vector2.right * _playerController.PlayerStats.ceilingCorrection, LayerReference.TerrainAndBoulder);

            if (ceilingCorrection != Vector2.zero && (ceilingCorrection.x.Sign0() * originalHorizontal.x.Sign0()) >= 0)
            {
                ForceOffset(ceilingCorrection);
                ledgeCorrected = true;
                correctedVertical = originalVertical;
            }
        }

        foreach (var hit in verticalHits.Concat(horizontalHits))
        {
            if (hit && hit.collider.GetComponent<ObjectMovementComponent>() is ObjectMovementComponent movementComponent)
            {
                if (hit.normal == Vector2.up)
                {
                    SetParentObject(movementComponent);
                }
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

                if (hitList.Any(hit => hit.distance * hit.fraction == 0))
                {
                    if (correctedVertical.y < 0)
                    {
                        ForceOffset(Vector2.up * _footColliderBounds.size.y);
                    }
                    else if (correctedVertical.y > 0)
                    {
                        oneWayCorrection += Vector2.up * _footColliderBounds.size.y;
                    }
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
                TimeLeftGround = Time.time;
                Grounded = false;
            }
        }
        else
        {
            if (combinedBaseVelocity.y < 0 && Mathf.Approximately(correctedVelocity.y, 0))
            {
                Grounded = true;
            }
        }


        if (correctedVelocity.x != 0)
        {
            LastHorizontalDirection = correctedVelocity.x.Sign0();
        }

        Velocity = correctedVelocity / Time.fixedDeltaTime - parentVelocity;

        Debug.DrawRay((Vector2)transform.position, correctedVelocity / 2, Color.green, 1);
        Debug.DrawRay((Vector2)transform.position + correctedVelocity / 2, correctedVelocity / 2, Color.yellow, 1);

        // Apply movement
        transform.localPosition = transform.localPosition + (Vector3)correctedVelocity;
    }

    public void SetParentObject(ObjectMovementComponent parent)
    {
        ParentObject = parent;
        //transform.SetParent(ParentObject?.transform);
    }

    private Vector2 GetParentVelocity()
    {
        if (ParentObject == null) return Vector2.zero;

        // if player hits parent or parent hits player, parent remains
        var hits = new List<RaycastHit2D>();

        var playerCollider = _playerController.CollisionController.MainCollider;
        var playerBounds = playerCollider.bounds;
        var parentCollider = ParentObject.GetComponent<BoxCollider2D>();
        var parentBounds = parentCollider.bounds;

        _playerController.CollisionController.CollideAndSlideVel(playerBounds.center, playerBounds, Velocity * Time.fixedDeltaTime, LayerReference.BoulderLayer, hits);
        ParentObject.CollideAndSlideVel(parentBounds.center, parentBounds, ParentObject.Velocity * Time.fixedDeltaTime,
        new ContactFilter2D() { useLayerMask = true, layerMask = LayerReference.PlayerLayer }, hits);

        if (!hits.Any(h => h))
        {
            SetParentObject(null);
            return Vector2.zero;
        }

        return ParentObject.Velocity;
    }

    void CheckAndFixOverlap()
    {
        var mainCollider = _playerController.CollisionController.MainCollider;
        Bounds detect = mainCollider.bounds;
        detect.Expand(-2 * _playerController.CollisionController.SkinWidth * Vector2.one);
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, detect.size, 0, Vector2.zero, 0, LayerReference.TerrainAndBoulder);
        if (hit)
        {
            ColliderDistance2D overlapdistance = Physics2D.Distance(mainCollider, hit.collider);
            Vector2 Correction = overlapdistance.pointB - overlapdistance.pointA;
            transform.position += (Vector3)Correction;
        }
    }

    Vector2 GetCorrection(Vector2 position, Vector2 originalDirection, Vector2 correctedDirection, Vector2 offsetA, Vector2 offsetB, LayerMask layerMask)
    {
        Vector2 positionA = position + offsetA;
        Vector2 positionB = position + offsetB;

        var collisionA = _collisonController.CollideAndSlideVel(positionA, MainColliderBounds, originalDirection, layerMask);
        var collisionB = _collisonController.CollideAndSlideVel(positionB, MainColliderBounds, originalDirection, layerMask);
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
