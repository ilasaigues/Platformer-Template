using System;
using System.Collections.Generic;
using System.Linq;
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
    public override void FixedUpdate(float delta)
    {
        var gravity = PlayerController.PlayerStats.fallGravity;
        PlayerController.MovementController.AddVelocity(gravity * delta * Vector2.up);

        var adjustedVelocity = PlayerController.CollisionController.CollideAndSlideVel(
            PlayerController.transform.position,
            delta * PlayerController.MovementController.Velocity.y * Vector2.up,
            LayerReference.TerrainLayer);
        PlayerController.MovementController.SetVelocity(null, adjustedVelocity.y / delta);
    }

    public override BehaviourChangeRequest VerifyBehaviour()
    {
        if (PlayerController.InputHandler.IsJumpPressed)
        {
            return BehaviourChangeRequest.New<PlayerJumpingBehaviour>();
        }
        Debug.DrawRay(PlayerController.transform.position, Vector2.down);
        if (!PlayerController.CollisionController.CheckCollision(
            PlayerController.transform.position,
            Vector2.down,
            LayerReference.TerrainLayer))
        {
            return BehaviourChangeRequest.New<PlayerFallingBehaviour>();
        }
        return null;
    }

    public override void Enter()
    {
    }

}
