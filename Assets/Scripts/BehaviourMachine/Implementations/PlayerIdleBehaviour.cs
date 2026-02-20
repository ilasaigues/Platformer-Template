using UnityEngine;

public class PlayerIdleBehaviour : PlayerGroundedBehaviour
{
    public PlayerIdleBehaviour(PlayerController player) : base(player)
    {

    }

    public override void Enter()
    {
        base.Enter();
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


    public override BehaviourChangeRequest VerifyBehaviour()
    {
        if (PlayerController.MovementController.Velocity.x != 0 || PlayerController.LastDirectionInput.x != 0)
        {        
            return BehaviourChangeRequest.New<PlayerGroundMoveBehaviour>();
        }

        return null;
    }

}