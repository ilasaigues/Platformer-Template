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
}
