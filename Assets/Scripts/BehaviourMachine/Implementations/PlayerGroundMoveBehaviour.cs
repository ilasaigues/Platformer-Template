using Unity.Mathematics;
using UnityEditor;
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
        float currentVelocity = PlayerController.MovementController.Velocity.x;
        float targetDirection = PlayerController.LastDirectionInput.x == 0 ? 0 : Mathf.Sign(PlayerController.LastDirectionInput.x);
        float maxSpeed = PlayerController.PlayerStats.groundedSpeed;


        // Check for wall collisions to prevent further movement using GetClosestCollisonSurfaceDistance

        float acceleration;
        if (targetDirection == 0 || currentVelocity.Sign0() * targetDirection.Sign0() == -1)
        {
            acceleration = maxSpeed / PlayerController.PlayerStats.groundedDecelerationTime * delta;
        }
        else
        {
            acceleration = maxSpeed / PlayerController.PlayerStats.groundedAccelerationTime * delta;
        }

        float difference = maxSpeed * targetDirection - PlayerController.MovementController.Velocity.x;
        acceleration = Mathf.Clamp(acceleration, 0, Mathf.Abs(difference)) * Mathf.Sign(difference);
        PlayerController.MovementController.AddVelocity(Vector2.right * acceleration);
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