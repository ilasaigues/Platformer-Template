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

    private ContactFilter2D TerrainAndBoulderFilter = new()
    {
        useLayerMask = true,
        layerMask = LayerReference.TerrainAndBoulder,
    };

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

        if (Velocity.y < _playerController.PlayerStats.fallVelocityCap)
        {
            SetVelocity(null, _playerController.PlayerStats.fallVelocityCap);
        }
        Vector2 originalVertical = Time.fixedDeltaTime * Velocity * Vector2.up;
        Vector2 originalHorizontal = Time.fixedDeltaTime * Velocity * Vector2.right;


        bool ledgeCorrected = false;

        var mainBounds = _collisonController.MainCollider.bounds;

        // Horizontal correction
        var horizontalHits = new List<RaycastHit2D>();
        var correctedHorizontal = BoxCaster2D.CollideAndSlideVel(mainBounds.center, mainBounds, originalHorizontal, LayerReference.TerrainAndBoulder.ToContactFilter2D(), horizontalHits);

        if (correctedHorizontal.magnitude < Mathf.Abs(originalHorizontal.x)) // if collided and shrunk vector
        {
            var ledgeCorrection = GetCorrection(Vector2.zero, mainBounds, originalHorizontal, correctedHorizontal, Vector2.up * _playerController.PlayerStats.ledgeCorrectionUp, Vector2.down * _playerController.PlayerStats.ledgeCorrectionDown, LayerReference.TerrainAndBoulder);

            if (ledgeCorrection != Vector2.zero)
            {
                ForceOffset(ledgeCorrection);
                ledgeCorrected = true;
                correctedHorizontal = originalHorizontal;
            }
        }


        // Vertical correction
        var verticalHits = new List<RaycastHit2D>();
        var correctedVertical = BoxCaster2D.CollideAndSlideVel(mainBounds.center + (Vector3)correctedHorizontal, mainBounds, originalVertical, LayerReference.TerrainAndBoulder.ToContactFilter2D(), verticalHits);

        if (!ledgeCorrected && !Grounded && originalVertical.y > 0 && correctedVertical.magnitude < Mathf.Abs(originalVertical.y)) // if collided and shrunk vector
        {
            var ceilingCorrection = GetCorrection(transform.position + (Vector3)correctedHorizontal, mainBounds, originalVertical, correctedVertical, Vector2.left * _playerController.PlayerStats.ceilingCorrection, Vector2.right * _playerController.PlayerStats.ceilingCorrection, LayerReference.TerrainAndBoulder);

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
        var footBounds = _collisonController.FootCollider.bounds;
        var hitList = new List<RaycastHit2D>();
        var oneWayCorrection = BoxCaster2D.CollideAndSlideVel(footBounds.center + (Vector3)correctedHorizontal, footBounds, originalVertical, OneWayFilter, hitList);

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

        var terrainAndBoulderFilter = LayerReference.TerrainAndBoulder.ToContactFilter2D();
        terrainAndBoulderFilter.useNormalAngle = true;
        terrainAndBoulderFilter.maxNormalAngle = 91;
        var groundHits = BoxCaster2D.GetHits(mainBounds.center + (Vector3)correctedVelocity, mainBounds, Vector2.down * Time.fixedDeltaTime, terrainAndBoulderFilter);
        var hitGround = groundHits.Any(h => h);
        if (Grounded)
        {
            if (!hitGround)
            {
                TimeLeftGround = Time.time;
                Grounded = false;
                SetParentObject(null);
            }
        }
        else
        {
            if (hitGround)
            {
                Grounded = true;
                SetParentObject(groundHits.Select(h => h.collider.GetComponent<ObjectMovementComponent>()).First());
            }
        }


        if (correctedVelocity.x != 0)
        {
            LastHorizontalDirection = correctedVelocity.x.Sign0();
        }

        CheckParenting();
        if (ParentObject != null)
            if (IsBeingSqueezed(ParentObject.Velocity * Time.fixedDeltaTime))
            {
                ParentObject.OnPlayerSqueezed(_playerController);
            }

        Velocity = correctedVelocity / Time.fixedDeltaTime;

        Debug.DrawRay((Vector2)transform.position, correctedVelocity / 2, Color.green, 1);
        Debug.DrawRay((Vector2)transform.position + correctedVelocity / 2, correctedVelocity / 2, Color.yellow, 1);

        // Apply movement
        transform.localPosition = transform.localPosition + (Vector3)correctedVelocity;
    }

    public void SetParentObject(ObjectMovementComponent parent)
    {
        ParentObject = parent;
        transform.SetParent(ParentObject?.transform);
    }

    public bool IsBeingSqueezed(Vector2 boulderVelocity)
    {

        var forwardHits = BoxCaster2D.GetHits(MainColliderBounds.center, MainColliderBounds, boulderVelocity, TerrainAndBoulderFilter)
        .Where(h => Vector2.Angle(h.normal, boulderVelocity) > 100);
        var backHits = BoxCaster2D.GetHits(MainColliderBounds.center, MainColliderBounds, -boulderVelocity, TerrainAndBoulderFilter)
        .Where(h => Vector2.Angle(h.normal, boulderVelocity) > 100);

        backHits = backHits.Where(bh => !forwardHits.Any(fh => fh.collider == bh.collider)).ToList();
        return forwardHits.Any(h => h) && backHits.Any(h => h);
    }

    private void CheckParenting()
    {
        if (ParentObject == null) return;

        // if player hits parent or parent hits player, parent remains
        var hits = new List<RaycastHit2D>();

        var playerCollider = _playerController.CollisionController.MainCollider;
        var playerBounds = playerCollider.bounds;
        var parentCollider = ParentObject.GetComponent<BoxCollider2D>();
        var parentBounds = parentCollider.bounds;

        hits.AddRange(BoxCaster2D.GetHits(playerBounds.center, playerBounds, Velocity * Time.fixedDeltaTime, LayerReference.BoulderLayer.ToContactFilter2D()));
        hits.AddRange(BoxCaster2D.GetHits(parentBounds.center, parentBounds, ParentObject.Velocity * Time.fixedDeltaTime, LayerReference.PlayerLayer.ToContactFilter2D()));
        hits.AddRange(BoxCaster2D.GetHits(playerBounds.center, playerBounds, Vector2.down * Time.fixedDeltaTime, LayerReference.BoulderLayer.ToContactFilter2D()));

        if (!hits.Any(h => h))
        {
            SetParentObject(null);
        }
    }

    void CheckAndFixOverlap()
    {
        var mainBounds = _playerController.CollisionController.MainCollider.bounds;
        List<RaycastHit2D> hits = BoxCaster2D.GetHits(mainBounds.center, mainBounds, Vector2.zero, LayerReference.TerrainAndBoulder.ToContactFilter2D());
        if (hits.Any(h => h))
        {
            ColliderDistance2D overlapdistance = Physics2D.Distance(_playerController.CollisionController.MainCollider, hits.First().collider);
            Vector2 Correction = overlapdistance.pointB - overlapdistance.pointA;
            transform.position += (Vector3)Correction;
        }
    }

    Vector2 GetCorrection(Vector2 pos, Bounds bounds, Vector2 originalDirection, Vector2 correctedDirection, Vector2 offsetA, Vector2 offsetB, LayerMask layerMask)
    {
        var collisionA = BoxCaster2D.CollideAndSlideVel(pos + offsetA, bounds, originalDirection, layerMask.ToContactFilter2D());
        var collisionB = BoxCaster2D.CollideAndSlideVel(pos + offsetB, bounds, originalDirection, layerMask.ToContactFilter2D());
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
