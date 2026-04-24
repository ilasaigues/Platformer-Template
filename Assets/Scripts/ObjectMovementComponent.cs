
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ObjectMovementComponent : MonoBehaviour
{
    public Vector2 Velocity { get; private set; }
    [Inject]
    private TimeContext _timeContext;

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
        _timeContext.CreateContextModules(gameObject);
    }

    void FixedUpdate()
    {
        if (Velocity == Vector2.zero || _timeContext.ContextTimescale.Value == 0) return;

        // check for collisions by velocity
        var mainBounds = _collider.bounds;

        var correctedVelocity = BoxCaster2D.CollideAndSlideVel(mainBounds.center, mainBounds, Velocity * _timeContext.FixedDeltaTime, LayerReference.TerrainLayer);

        if (correctedVelocity.magnitude + 1e-6 < Velocity.magnitude * _timeContext.FixedDeltaTime)
        {
            OnObstacleHit();
        }

        List<RaycastHit2D> collisionsWithPlayer = BoxCaster2D.GetHits(mainBounds.center, mainBounds, _timeContext.FixedDeltaTime * Velocity, LayerReference.PlayerLayer);

        collisionsWithPlayer.AddRange(BoxCaster2D.GetHits(mainBounds.center, mainBounds, Vector2.up * _timeContext.FixedDeltaTime, LayerReference.PlayerLayer, 1));

        if (collisionsWithPlayer.Any(c => c))
        {
            var playerHit = collisionsWithPlayer.First(c => c);
            var playerController = playerHit.collider.GetComponent<PlayerController>();
            playerController.MovementController.ExternalVelocity = Velocity;

            var directionToPlayer = playerController.transform.position - transform.position;
            var movingAgainstPlayer = Vector2.Dot(directionToPlayer.normalized, Velocity.normalized) > 0;

            List<BreakableTerrainBehaviour> breakables = new();

            if (movingAgainstPlayer && IsSqueezingPlayer(playerController.CollisionController.MainCollider.bounds, breakables))
            {
                if (playerController.MovementController.CanBeSqueezed)
                {
                    OnPlayerSqueezed(playerController);
                }
                else
                {
                    if (breakables.Count > 0)
                    {
                        breakables.ForEach(b => b.Break());
                    }
                    else
                    {
                        correctedVelocity = BoxCaster2D.CollideAndSlideVel(mainBounds.center, mainBounds, Velocity * _timeContext.FixedDeltaTime, LayerReference.TerrainAndPlayer);
                        OnObstacleHit();
                        playerController.MovementController.ExternalVelocity = Vector2.zero;
                    }
                }
            }
        }

        transform.position = transform.position + (Vector3)correctedVelocity;
        FixPlayerOverlap();
    }

    public void FixPlayerOverlap()
    {
        var mainBounds = _collider.bounds;
        List<RaycastHit2D> hits = BoxCaster2D.GetHits(mainBounds.center, mainBounds, Vector2.zero, LayerReference.PlayerLayer);
        if (hits.Any(h => h))
        {
            ColliderDistance2D overlapdistance = Physics2D.Distance(_collider, hits.First().collider);
            Vector2 Correction = -overlapdistance.normal * overlapdistance.distance;
            hits.First().collider.GetComponent<MovementController>().ForceOffset(Correction);
        }
    }

    public bool IsSqueezingPlayer(Bounds playerBounds, List<BreakableTerrainBehaviour> breakables)
    {
        var castVector = Velocity * _timeContext.FixedDeltaTime;

        var forwardHits = BoxCaster2D.GetHits(playerBounds.center, playerBounds, castVector, LayerReference.TerrainAndBoulder, 1);
        var backHits = BoxCaster2D.GetHits(playerBounds.center, playerBounds, -castVector, LayerReference.TerrainAndBoulder, 1);

        backHits = backHits.Where(bh => !forwardHits.Any(fh => fh.collider == bh.collider)).ToList();

        breakables.AddRange(forwardHits.Concat(backHits).Select(hit => hit.collider.GetComponent<BreakableTerrainBehaviour>()).Where(btb => btb != null));

        return backHits.Any(hit => hit) && forwardHits.Any(hit => hit);
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