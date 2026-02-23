using UnityEngine;

public abstract class PlayerAirBehaviour : BasePlayerBehaviour
{
    public PlayerAirBehaviour(PlayerController playerController) : base(playerController) { }
    public override BehaviourChangeRequest VerifyBehaviour()
    {
        var verticalVelocity = PlayerController.MovementController.Velocity.y;
        // REDO
        Debug.DrawRay(PlayerController.transform.position, PlayerController.MovementController.Velocity * Time.fixedDeltaTime, Color.red);
        if (verticalVelocity <= 0 &&  // going down and touching the ground
         PlayerController.CollisionController.CheckCollision(
            PlayerController.transform.position,
            PlayerController.MovementController.Velocity * Time.fixedDeltaTime,
            LayerReference.TerrainLayer))
        {
            return new BehaviourChangeRequest() { NewBehaviourType = typeof(PlayerIdleBehaviour) };
        }

        return null;
    }

    public override void FixedUpdate(float delta)
    {
        var gravity = CurrentGravity();
        PlayerController.MovementController.AddVelocity(gravity * delta * Vector2.up);


        var adjustedVelocity = PlayerController.CollisionController.CollideAndSlideVel(
            PlayerController.transform.position,
            delta * PlayerController.MovementController.Velocity.y * Vector2.up,
            LayerReference.TerrainLayer);
        PlayerController.MovementController.SetVelocity(null, adjustedVelocity.y / delta);
    }

    public abstract float CurrentGravity();
}
