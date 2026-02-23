using UnityEngine;

public class PlayerIdleBehaviour : PlayerGroundedBehaviour
{
    public PlayerIdleBehaviour(PlayerController player) : base(player)
    {

    }

    public override void Enter()
    {
        base.Enter();
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
        //throw new System.NotImplementedException();
    }


    public override BehaviourChangeRequest VerifyBehaviour()
    {
        var baseValue = base.VerifyBehaviour();
        if (baseValue != null) return baseValue;
        if (PlayerController.MovementController.Velocity.x != 0 || PlayerController.LastDirectionInput.x != 0)
        {
            return BehaviourChangeRequest.New<PlayerGroundMoveBehaviour>();
        }

        return null;
    }

}