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
        if (PlayerController.InputHandler.JumpButton.JustPressed &&
           (DateTime.Now - PlayerController.MovementController.TimeLeftGround).TotalSeconds <=
           PlayerController.PlayerStats.coyoteTime &&
           PlayerController.TryJump())
        {
            return BehaviourChangeRequest.New<PlayerJumpingBehaviour>();
        }
        return null;
    }
}