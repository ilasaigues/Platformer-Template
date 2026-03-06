using UnityEngine;

public abstract class PlayerAirBehaviour : BasePlayerBehaviour
{
    public PlayerAirBehaviour(PlayerController playerController) : base(playerController) { }
    public override BehaviourChangeRequest VerifyBehaviour()
    {
        if (base.VerifyBehaviour() is BehaviourChangeRequest baseBehaviour)
        {
            return baseBehaviour;
        }

        var verticalVelocity = PlayerController.MovementController.Velocity.y;
        // REDO



        if (verticalVelocity <= 0 &&
            PlayerController.MovementController.Grounded)
        {
            return BehaviourChangeRequest.New<PlayerIdleBehaviour>();
        }

        return null;
    }

    public override void FixedUpdate(float delta)
    {
        var gravity = CurrentGravity();
        PlayerController.MovementController.AddVelocity(gravity * delta * Vector2.up);

        float currentVelocity = PlayerController.MovementController.Velocity.x;
        int targetDirection = PlayerController.LastDirectionInput.x.Sign0();
        float maxSpeed = PlayerController.PlayerStats.airSpeed;

        float acceleration;
        if (targetDirection == 0 || currentVelocity.Sign0() * targetDirection == -1)
        {
            acceleration = maxSpeed / PlayerController.PlayerStats.airDecelerationTime * delta;
        }
        else
        {
            acceleration = maxSpeed / PlayerController.PlayerStats.airAccelerationTime * delta;
        }

        float difference = maxSpeed * targetDirection - currentVelocity;
        acceleration = Mathf.Clamp(acceleration, 0, Mathf.Abs(difference)) * Mathf.Sign(difference);

        PlayerController.SetSpriteDirection(targetDirection);
        PlayerController.MovementController.AddVelocity(acceleration * Vector2.right);
    }

    public abstract float CurrentGravity();
}
