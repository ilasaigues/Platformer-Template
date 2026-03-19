using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerPlatformAttacher : MonoBehaviour
{
    public bool IsOnMovingPlatform;
    public bool UnSqueezable;

    // handle if is on moving platform

    // handle updating player external velocity while attached

    // handle moving and squeezing

    public event Action OnPlayerSqueezed = delegate { };


    public ObjectMovementComponent AttachedParent;

    private ContactFilter2D TerrainAndBoulderFilter = new()
    {
        useLayerMask = true,
        layerMask = LayerReference.TerrainLayer | LayerReference.BoulderLayer,
    };
    private PlayerController _playerController;

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
    }

    public void AttachComponent(ObjectMovementComponent attachingComponent)
    {
        Debug.Log("ATTACHING");

        AttachedParent = attachingComponent;
    }

    public void DetachComponent()
    {
        Debug.Log("DETACHING");
        AttachedParent = null;
    }


    public bool IsBeingSqueezed(Vector2 squeezeVelocity)
    {
        var playerColliderBounds = _playerController.CollisionController.MainCollider.bounds;
        List<RaycastHit2D> forwardHits = new();
        _playerController.CollisionController.CollideAndSlideVel(playerColliderBounds.center, playerColliderBounds, squeezeVelocity, TerrainAndBoulderFilter, forwardHits);
        List<RaycastHit2D> backHits = new();
        _playerController.CollisionController.CollideAndSlideVel(playerColliderBounds.center, playerColliderBounds, -squeezeVelocity, TerrainAndBoulderFilter, backHits);
        bool squeezed = forwardHits.Any(c => c) && backHits.Any(c => c);

        if (squeezed && !UnSqueezable)
        {
            OnPlayerSqueezed();
        }

        return squeezed;
    }

    public Vector2 CorrectVelocity(Vector2 playerFrameVelocity, bool vertical)
    {
        if (AttachedParent != null)
        {
            var parentFrameVelocity = (vertical ? Vector2.up * AttachedParent.Velocity.y : Vector2.right * AttachedParent.Velocity.x) * Time.fixedDeltaTime;
            FixPlayerOverlapCorrection(transform.position, parentFrameVelocity, vertical);
            if (playerFrameVelocity != Vector2.zero)
            {
                var dotProduct = Vector2.Dot(playerFrameVelocity.normalized, parentFrameVelocity.normalized);
                if (dotProduct > 0 && playerFrameVelocity.magnitude > parentFrameVelocity.magnitude) // same direction, faster than parent
                {
                    DetachComponent();
                    return playerFrameVelocity;
                }
                else //same direction, slower than parent OR opposite direction
                {
                    return parentFrameVelocity;
                }
            }
            else
            {
                var platformToPlayer = _playerController.transform.position - AttachedParent.transform.position;
                if (Vector2.Dot(parentFrameVelocity.normalized, platformToPlayer.normalized) >= 0) // moving towards player
                {
                    return parentFrameVelocity;
                }
                else // moving away from player
                {
                    DetachComponent();
                    return playerFrameVelocity;
                }
            }
        }
        return playerFrameVelocity;
    }

    void FixPlayerOverlapCorrection(Vector2 playerPosition, Vector2 platformDirection, bool vertical)
    {
        // raycast from playerPos in platformdirection
        var hit = Physics2D.Raycast(playerPosition, -platformDirection, 1, LayerReference.BoulderLayer);
        if (hit)
        {
            var playerBounds = _playerController.CollisionController.MainCollider.bounds;
            if (vertical)
            {
                _playerController.MovementController.ForcePosition(hit.point + platformDirection.normalized * playerBounds.size.y / 2);
            }
            else
            {
                _playerController.MovementController.ForcePosition(hit.point + platformDirection.normalized * playerBounds.size.x / 2);
            }
            // ensure player position is half it's width + point if horizontal, half it's height + point if vertical
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.collider.name);
    }

}
