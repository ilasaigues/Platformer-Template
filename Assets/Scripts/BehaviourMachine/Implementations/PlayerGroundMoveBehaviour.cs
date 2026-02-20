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
        
        float targetDirection = PlayerController.LastDirectionInput.x == 0 ? 0 : Mathf.Sign(PlayerController.LastDirectionInput.x);
        float targetSpeed = PlayerController.PlayerStats.groundedSpeed;
        float acceleration = targetSpeed/(targetDirection == 0 ? PlayerController.PlayerStats.groundedDecelerationTime : PlayerController.PlayerStats.groundedAccelerationTime) * delta;
        float difference = targetSpeed * targetDirection - PlayerController.MovementController.Velocity.x;
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