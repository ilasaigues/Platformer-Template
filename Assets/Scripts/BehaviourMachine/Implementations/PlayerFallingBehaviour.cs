using System;

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
            bool canDoubleJump = PlayerController.MaxJumps > 1 && PlayerController.RemainingJumps < PlayerController.MaxJumps; // I have an extra jump
            bool isInCoyoteTime = (DateTime.Now - PlayerController.MovementController.TimeLeftGround).TotalSeconds <=
               PlayerController.PlayerStats.coyoteTime;
            if ((isInCoyoteTime && PlayerController.TryJump()) || (canDoubleJump && PlayerController.TryJump()))
            {
                return BehaviourChangeRequest.New<PlayerJumpingBehaviour>();
            }
        }
        return null;
    }
}