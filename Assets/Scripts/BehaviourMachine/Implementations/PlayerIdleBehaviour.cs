using UnityEngine;

public class PlayerIdleBehaviour : PlayerGroundedBehaviour
{
    public PlayerIdleBehaviour(PlayerController player) : base(player)
    {

    }

    public override void Enter()
    {
        PlayerController.MovementController.SetVelocity(
            new Vector2(PlayerController.MovementController.Velocity.x, 0));
    }

    public override void Exit()
    {
        //throw new System.NotImplementedException();
    }

    public override void FixedUpdate(float delta)
    {
        //throw new System.NotImplementedException();
    }

    public override void Update(float delta)
    {
        //throw new System.NotImplementedException();
    }

    public override BehaviourChangeRequest VerifyBehaviour()
    {
        //throw new System.NotImplementedException();
        return null;
    }
}