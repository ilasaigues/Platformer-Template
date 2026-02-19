using UnityEngine;

public class PlayerGroundMoveBehaviour : PlayerGroundedBehaviour
{


    public PlayerGroundMoveBehaviour(PlayerController player) : base(player)
    {
    }

    public override void Exit()
    {
        _lastHorizontalInput = 0;
    }

    public override void FixedUpdate(float delta)
    {
        if (_lastHorizontalInput != 0) // input is being pressed
        {
            var maxDeltaV = PlayerController.MovementController.Velocity.x - PlayerController.PlayerStats.groundedSpeed;
            var inputDeltaV = _lastHorizontalInput * delta * PlayerController.PlayerStats.groundedAcceleration;

            PlayerController.MovementController.AddVelocity(Mathf.Min(maxDeltaV, inputDeltaV) * Vector2.right);
        }
        else // input is not being pressed
        {
            var currentVelocity = PlayerController.MovementController.Velocity.x;
            float deceleration = PlayerController.PlayerStats.groundedDeceleration;
            if (currentVelocity > 0) // negative velocity
            {
                if (currentVelocity - deceleration < 0) // overshoot, lower deceleration
                {
                    deceleration = -currentVelocity;
                }
            }
            else if (currentVelocity < 0) // positive velocity
            {
                if (currentVelocity + deceleration > 0) // overshoot, lower deceleration
                {
                    deceleration = -currentVelocity;
                }
            }
            PlayerController.MovementController.AddVelocity(deceleration * Vector2.right);
        }
    }

    public override BehaviourChangeRequest VerifyBehaviour()
    {
        if (PlayerController.MovementController.Velocity.x == 0)
        {
            return BehaviourChangeRequest.New<PlayerIdleBehaviour>();
        }
        return null;
    }
}