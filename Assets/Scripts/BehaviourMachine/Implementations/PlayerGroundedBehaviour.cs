using UnityEngine;

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
        /*var gravity = PlayerController.PlayerStats.fallGravity;
        PlayerController.MovementController.AddVelocity(gravity * delta * Vector2.up);*/
    }

    public override BehaviourChangeRequest VerifyBehaviour()
    {
        if (base.VerifyBehaviour() is BehaviourChangeRequest baseBehaviour)
        {
            return baseBehaviour;
        }

        if (PlayerController.InputHandler.JumpButton.JustPressed ||
            PlayerController.InputHandler.JumpButton.TimeSinceLastPressed <=
            PlayerController.PlayerStats.jumpBufferTime)
        {
            return BehaviourChangeRequest.New<PlayerJumpingBehaviour>();
        }
        if (!PlayerController.MovementController.Grounded)
        {
            return BehaviourChangeRequest.New<PlayerFallingBehaviour>();
        }
        return null;
    }

    public override void Enter()
    {
        PlayerController.ResetOnGrounded();
    }

}
