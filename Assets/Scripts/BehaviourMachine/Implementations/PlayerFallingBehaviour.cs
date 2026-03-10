using System;
using UnityEngine;
public class PlayerFallingBehaviour : PlayerAirBehaviour
{
    public PlayerFallingBehaviour(PlayerController playerController) : base(playerController)
    {
    }

    public override float CurrentGravity()
    {
        return PlayerController.PlayerStats.fallGravity;
    }

    public override void Enter()
    {
        EnqueueAnim(PlayerController.PlayerAnimator.AnimationList.Peak);
    }

    public override void Exit()
    {
    }

    public override void FixedUpdate(float delta)
    {
        base.FixedUpdate(delta);

    }

    public override void Update(float delta)
    {
    }

    public override BehaviourChangeRequest VerifyBehaviour()
    {
        var baseValue = base.VerifyBehaviour();
        if (baseValue != null) return baseValue;


        bool jumpRequested = PlayerController.InputHandler.JumpButton.JustPressed;
        if (jumpRequested)
        {
            bool isInCoyoteTime = Time.time - PlayerController.MovementController.TimeLeftGround <=
               PlayerController.PlayerStats.coyoteTime;
            if (isInCoyoteTime && PlayerController.Jumps < 1)
            {
                return BehaviourChangeRequest.New<PlayerJumpingBehaviour>();
            }
            else if (PlayerController.Jumps < 2)
            {
                return BehaviourChangeRequest.New<PlayerDoubleJumpBehaviour>();
            }
        }
        return null;
    }
}