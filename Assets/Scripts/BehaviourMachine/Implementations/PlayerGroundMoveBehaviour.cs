using Unity.Mathematics;
using UnityEngine;

public class PlayerGroundMoveBehaviour : PlayerGroundedBehaviour
{


    public PlayerGroundMoveBehaviour(PlayerController player) : base(player)
    {
    }

    public override void Exit()
    {

    }

    public override void FixedUpdate(float delta)
    {
        var inputSign = PlayerController.LastDirectionInput.x == 0 ? 0 : Mathf.Sign(PlayerController.LastDirectionInput.x);
        var targetHorizontalVelocity = inputSign * PlayerController.PlayerStats.groundedSpeed;
        var currentVelocity = PlayerController.MovementController.Velocity.x;
        if (targetHorizontalVelocity != 0) // target velocity is NOT zero
        {
            var inputAcceleration = delta * Mathf.Sign(targetHorizontalVelocity) * PlayerController.PlayerStats.groundedAcceleration;
            if (currentVelocity + inputAcceleration > targetHorizontalVelocity)
            {
                inputAcceleration = targetHorizontalVelocity - currentVelocity;
            }

            PlayerController.MovementController.AddVelocity(inputAcceleration * Vector2.right);
        }
        else // target velocity is zero
        {
            float deceleration = PlayerController.PlayerStats.groundedDeceleration * delta;
            if (currentVelocity > 0) // negative deceleration
            {
                deceleration *= -1;
            }
            if (Mathf.Abs(deceleration) > Mathf.Abs(currentVelocity)) // overshoot, lower deceleration
            {
                deceleration = -currentVelocity;
            }

            PlayerController.MovementController.AddVelocity(deceleration * Vector2.right);
        }
        // todo: CHOPPY MOVEMENT TO THE LEFT, ALSO, DECELERATION SHOULD APPLY IF TARGET VELOCITY IS OPPOSITE OF CURRENT
    }

    public override BehaviourChangeRequest VerifyBehaviour()
    {
        if (PlayerController.MovementController.Velocity.x == 0 && PlayerController.LastDirectionInput.x == 0)
        {
            return BehaviourChangeRequest.New<PlayerIdleBehaviour>();
        }
        return null;
    }
}