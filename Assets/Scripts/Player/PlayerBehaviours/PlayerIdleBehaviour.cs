using UnityEngine;

public class PlayerIdleBehaviour : PlayerGroundedBehaviour
{
    public PlayerIdleBehaviour(PlayerController player) : base(player)
    {

    }

    public override void Enter()
    {
        base.Enter();
        EnqueueAnim(PlayerController.PlayerAnimator.AnimationList.Idle);
    }

    public override void Exit()
    {
        //throw new System.NotImplementedException();
    }

    public override void Update(float delta)
    {

    }

    public override void FixedUpdate(float delta)
    {
        base.FixedUpdate(delta);

        float currentVelocity = PlayerController.MovementController.Velocity.x;
        int targetDirection = PlayerController.LastDirectionInput.x.Sign0();
        float maxSpeed = PlayerController.PlayerStats.groundedSpeed;

        float acceleration = maxSpeed / PlayerController.PlayerStats.groundedDecelerationTime * delta;

        float difference = maxSpeed * targetDirection - currentVelocity;
        acceleration = Mathf.Clamp(acceleration, 0, Mathf.Abs(difference)) * Mathf.Sign(difference);

        PlayerController.SetSpriteDirection(targetDirection);

        PlayerController.MovementController.AddVelocity(acceleration * Vector2.right);
    }


    public override BehaviourChangeRequest VerifyBehaviour()
    {
        var baseValue = base.VerifyBehaviour();
        if (baseValue != null) return baseValue;
        if (PlayerController.LastDirectionInput.x != 0)
        {
            return BehaviourChangeRequest.New<PlayerGroundMoveBehaviour>();
        }

        return null;
    }

}