using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerGroundedBehaviour : BasePlayerBehaviour
{

    protected PlayerGroundedBehaviour(PlayerController player) : base(player)
    {
    }


    public override void Update(float delta)
    {
    }

    public override void Enter()
    {
        // TODO: Convert this collison check in to a generic function that takes any direction for reuse
        // check distance to ground vs collider extents on left and right side
        var playerLeft = PlayerController.transform.position;
        var playerRight = PlayerController.transform.position;
        playerLeft.x -= PlayerController.ColliderController.ColliderBounds.extents.x;
        playerRight.x += PlayerController.ColliderController.ColliderBounds.extents.x;
        var colliderMin = PlayerController.ColliderController.ColliderBounds.min;
        var hitLeft = PlayerController.CollisionController.CheckCollision(playerLeft, Vector2.down, 10, LayerReference.TerrainLayer);
        var hitRight = PlayerController.CollisionController.CheckCollision(playerRight, Vector2.down, 10, LayerReference.TerrainLayer);
        // calculate offset based on HIGHEST collision point (max)
        if (hitLeft.collider != null || hitRight.collider != null)
        {
            var offset = Vector2.up * (Mathf.Max(hitLeft.point.y, hitRight.point.y) - colliderMin.y);
            PlayerController.MovementController.ForceOffset(offset);
        }

    }

}
