using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Zenject;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CollisionController))]
[RequireComponent(typeof(PlayerController))]
public class MovementController : MonoBehaviour
{
    public Vector2 Velocity { get; private set; }

    public Vector2 ExternalVelocity;

    public bool Grounded = false;
    public bool OnOneWayPlatform = false;
    public int LastHorizontalDirection;

    public bool CanBeSqueezed = true;


    public float TimeLeftGround;

    public bool IgnoreOneWay = false;

    [Inject]
    private TimeContext _timeContext;
    private Rigidbody2D _rb;
    private CollisionController _collisonController;
    private PlayerController _playerController;

    public Bounds MainColliderBounds => _collisonController.MainCollider.bounds;
    private Bounds _footColliderBounds => _collisonController.FootCollider.bounds;

    public ObjectMovementComponent ParentObject { get; private set; }

    public float VerticalTerminalVelocity;


    void Awake()
    {
        CanBeSqueezed = true;
        _rb = gameObject.GetOrAddComponent<Rigidbody2D>();
        _collisonController = gameObject.GetOrAddComponent<CollisionController>();
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _playerController = gameObject.GetOrAddComponent<PlayerController>();
        VerticalTerminalVelocity = _playerController.PlayerStats.fallVelocityCap;
    }

    void Start()
    {
        _timeContext.CreateContextModules(gameObject);
    }

    void FixedUpdate()
    {
        CheckAndFixOverlap();
        if (_timeContext.ContextTimescale.Value == 0) return;

        ForceOffset(ExternalVelocity * _timeContext.FixedDeltaTime);
        ExternalVelocity = Vector2.zero;

        if (Velocity.y < VerticalTerminalVelocity)
        {
            SetVelocity(null, VerticalTerminalVelocity);
        }

        Vector2 originalVertical = _timeContext.FixedDeltaTime * Velocity * Vector2.up;
        Vector2 originalHorizontal = _timeContext.FixedDeltaTime * Velocity * Vector2.right;


        bool ledgeCorrected = false;

        var mainBounds = _collisonController.MainCollider.bounds;

        // Horizontal correction
        var horizontalHits = new List<RaycastHit2D>();
        var correctedHorizontal = BoxCaster2D.CollideAndSlideVel(mainBounds.center, mainBounds, originalHorizontal, LayerReference.TerrainAndBoulder, horizontalHits);

        if (!Grounded && correctedHorizontal.magnitude < Mathf.Abs(originalHorizontal.x)) // if collided and shrunk vector
        {
            var ledgeCorrection = GetCorrection(transform.position, mainBounds, originalHorizontal, correctedHorizontal, Vector2.up * _playerController.PlayerStats.ledgeCorrectionUp, Vector2.down * _playerController.PlayerStats.ledgeCorrectionDown, LayerReference.TerrainAndBoulder);

            if (ledgeCorrection != Vector2.zero)
            {
                ForceOffset(ledgeCorrection);
                ledgeCorrected = true;
                correctedHorizontal = originalHorizontal;
            }
        }


        // Vertical correction
        var verticalHits = new List<RaycastHit2D>();
        var correctedVertical = BoxCaster2D.CollideAndSlideVel(mainBounds.center + (Vector3)correctedHorizontal, mainBounds, originalVertical, LayerReference.TerrainAndBoulder, verticalHits);

        if (!Grounded && !ledgeCorrected && !Grounded && originalVertical.y > 0 && correctedVertical.magnitude < Mathf.Abs(originalVertical.y)) // if collided and shrunk vector
        {
            var ceilingCorrection = GetCorrection(transform.position + (Vector3)correctedHorizontal, mainBounds, originalVertical, correctedVertical, Vector2.left * _playerController.PlayerStats.ceilingCorrection, Vector2.right * _playerController.PlayerStats.ceilingCorrection, LayerReference.TerrainAndBoulder);

            if (ceilingCorrection != Vector2.zero && (ceilingCorrection.x.Sign0() * originalHorizontal.x.Sign0()) >= 0)
            {
                ForceOffset(ceilingCorrection);
                ledgeCorrected = true;
                correctedVertical = originalVertical;
            }
        }
        horizontalHits.ForEach(hit => Debug.DrawRay(hit.point, hit.normal, Color.blue, 1));
        verticalHits.ForEach(hit => Debug.DrawRay(hit.point, hit.normal, Color.red, 1));
        /*foreach (var hit in verticalHits.Concat(horizontalHits))
        {
            if (hit && hit.collider.GetComponent<ObjectMovementComponent>() is ObjectMovementComponent movingPlatform)
            {
                if (hit.normal == Vector2.up)
                {
                    SetParentObject(movementComponent);
                }
            }
        }*/

        // One Way Correction
        var footBounds = _collisonController.FootCollider.bounds;
        var hitList = new List<RaycastHit2D>();
        var oneWayCorrection = BoxCaster2D.CollideAndSlideVel(footBounds.center + (Vector3)correctedHorizontal, footBounds, originalVertical, LayerReference.OneWayPlatformLayer, hitList);
        if (originalVertical.y == 0) // edge case for foot check
        {
            hitList = BoxCaster2D.GetHits(footBounds.center + (Vector3)correctedHorizontal, footBounds, Vector2.down * _footColliderBounds.size.y, LayerReference.OneWayPlatformLayer);
        }
        var hitPlatformFromAbove = hitList.Count != 0 && hitList.Any(hit => hit.normal.y > 0.5f);
        if (hitPlatformFromAbove)
        {
            if (!IgnoreOneWay)
            {
                OnOneWayPlatform = true;
                if (hitList.Any(hit => hit.distance * hit.fraction == 0))
                {
                    if (correctedVertical.y < 0) // going down, force offset up
                    {
                        ForceOffset(Vector2.up * _footColliderBounds.size.y);
                    }
                    else if (correctedVertical.y > 0) // going up, add a little bit of vertical movement
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


        var groundHits = BoxCaster2D.GetHits(mainBounds.center + (Vector3)correctedVelocity, mainBounds, Vector2.down * _timeContext.FixedDeltaTime, LayerReference.TerrainAndBoulder);
        groundHits = groundHits.Where(h => Vector2.Angle(h.normal, Vector2.up) == 0).ToList();
        var hitGround = groundHits.Any(h => h) || OnOneWayPlatform;
        // todo: check if platform hit
        if (Grounded)
        {
            if (!hitGround)
            {
                TimeLeftGround = _timeContext.Time;
                Grounded = false;
            }
        }
        else
        {
            if (hitGround)
            {
                Grounded = true;
            }
        }


        if (correctedVelocity.x != 0)
        {
            LastHorizontalDirection = correctedVelocity.x.Sign0();
        }

        Velocity = correctedVelocity / _timeContext.FixedDeltaTime;

        Debug.DrawRay((Vector2)transform.position, correctedVelocity / 2, Color.green, 1);
        Debug.DrawRay((Vector2)transform.position + correctedVelocity / 2, correctedVelocity / 2, Color.yellow, 1);


        // Apply movement
        transform.localPosition = transform.localPosition + (Vector3)correctedVelocity;
    }

    void CheckAndFixOverlap()
    {
        var mainBounds = _playerController.CollisionController.MainCollider.bounds;
        List<RaycastHit2D> hits = BoxCaster2D.GetHits(mainBounds.center, mainBounds, Vector2.zero, LayerReference.TerrainLayer);
        if (hits.Any(h => h))
        {
            ColliderDistance2D overlapdistance = Physics2D.Distance(_playerController.CollisionController.MainCollider, hits.First().collider);
            Vector2 Correction = overlapdistance.pointB - overlapdistance.pointA;
            transform.position += (Vector3)Correction;
        }
    }

    Vector2 GetCorrection(Vector2 pos, Bounds bounds, Vector2 originalDirection, Vector2 correctedDirection, Vector2 offsetA, Vector2 offsetB, LayerMask layerMask)
    {
        List<RaycastHit2D> aHits = new();
        var collisionA = BoxCaster2D.CollideAndSlideVel(pos + offsetA, bounds, originalDirection, layerMask, aHits);
        List<RaycastHit2D> bHits = new();
        var collisionB = BoxCaster2D.CollideAndSlideVel(pos + offsetB, bounds, originalDirection, layerMask, bHits);
        if (collisionA == collisionB)
        {
            return Vector2.zero;
        }

        if (aHits.Count == 0 && collisionA.magnitude > correctedDirection.magnitude)
        {
            DebugExtensions.DrawBox(bHits[0].point, 0.2f, Color.green);
            return offsetA;
        }
        else if (bHits.Count == 0 && collisionB.magnitude > correctedDirection.magnitude)
        {
            DebugExtensions.DrawBox(aHits[0].point, 0.2f, Color.red);
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
