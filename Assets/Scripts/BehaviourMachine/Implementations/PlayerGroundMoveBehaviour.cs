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
        base.FixedUpdate(delta);
        float currentVelocity = PlayerController.MovementController.Velocity.x;
        float targetDirection = PlayerController.LastDirectionInput.x.Sign0();
        float maxSpeed = PlayerController.PlayerStats.groundedSpeed;

        float acceleration;
        if (targetDirection == 0 || currentVelocity.Sign0() * targetDirection.Sign0() == -1)
        {
            acceleration = maxSpeed / PlayerController.PlayerStats.groundedDecelerationTime * delta;
        }
        else
        {
            acceleration = maxSpeed / PlayerController.PlayerStats.groundedAccelerationTime * delta;
        }

        float difference = maxSpeed * targetDirection - currentVelocity;
        acceleration = Mathf.Clamp(acceleration, 0, Mathf.Abs(difference)) * Mathf.Sign(difference);

        PlayerController.MovementController.AddVelocity(acceleration * Vector2.right);

        /*var adjustedVelocity = PlayerController.CollisionController.CollideAndSlideVel(
            PlayerController.transform.position,
            delta * PlayerController.MovementController.Velocity.x * Vector2.right,
            LayerReference.TerrainLayer);
        PlayerController.MovementController.SetVelocity(adjustedVelocity.x / delta, null);*/

    }


    public override BehaviourChangeRequest VerifyBehaviour()
    {
        var baseValue = base.VerifyBehaviour();
        if (baseValue != null) return baseValue;
        if (PlayerController.MovementController.Velocity.x == 0 && PlayerController.LastDirectionInput.x == 0)
        {
            return BehaviourChangeRequest.New<PlayerIdleBehaviour>();
        }
        return null;
    }
}