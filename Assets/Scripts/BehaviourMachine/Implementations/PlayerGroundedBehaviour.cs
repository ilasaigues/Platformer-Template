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
    }

    public override BehaviourChangeRequest VerifyBehaviour()
    {
        if ((PlayerController.InputHandler.JumpButton.JustPressed ||
            PlayerController.InputHandler.JumpButton.TimeSinceLastPressed.TotalSeconds <=
            PlayerController.PlayerStats.jumpBufferTime) &&
            PlayerController.TryJump())
        {
            return BehaviourChangeRequest.New<PlayerJumpingBehaviour>();
        }
        Debug.DrawRay(PlayerController.transform.position, Vector2.down);
        if (!PlayerController.MovementController.Grounded)
        {
            return BehaviourChangeRequest.New<PlayerFallingBehaviour>();
        }
        return null;
    }

    public override void Enter()
    {
        PlayerController.ResetJumps();
    }

}
