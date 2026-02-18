using UnityEngine;

public abstract class PlayerAirBehaviour : BasePlayerBehaviour
{
    public PlayerAirBehaviour(PlayerController playerController) : base(playerController) { }
    public override BehaviourChangeRequest VerifyBehaviour()
    {
        var verticalVelocity = PlayerController.MovementController.Velocity.y;
        if (verticalVelocity <= 0 &&  // going down and touching the ground
         PlayerController.CollisionController.GetVerticalCollisions(Vector2.down, verticalVelocity * Time.fixedDeltaTime, 1 << 8).Count > 0)
        {
            return new BehaviourChangeRequest() { NewBehaviourType = typeof(PlayerIdleBehaviour) };
        }

        return null;
    }

    public override void FixedUpdate(float delta)
    {
        var gravity = CurrentGravity();

        PlayerController.MovementController.AddVelocity(delta * gravity * Vector2.up);
        if (PlayerController.MovementController.Velocity.y > PlayerController.PlayerStats.fallVelocityCap) // max air velocity correction
        {
            PlayerController.MovementController.AddVelocity((PlayerController.PlayerStats.fallVelocityCap - PlayerController.MovementController.Velocity.y) * Vector2.up * delta);
        }
    }

    public abstract float CurrentGravity();
}
