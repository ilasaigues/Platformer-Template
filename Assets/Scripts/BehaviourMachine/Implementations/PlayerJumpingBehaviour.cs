using UnityEngine;
public class PlayerJumpingBehaviour : PlayerAirBehaviour

{
    public PlayerJumpingBehaviour(PlayerController playerController) : base(playerController)
    {
    }

    public override BehaviourChangeRequest VerifyBehaviour()
    {
        var newBehaviuourRequest = base.VerifyBehaviour();
        if (newBehaviuourRequest != null) return newBehaviuourRequest;
        if (PlayerController.MovementController.Velocity.y < PlayerController.PlayerStats.peakTresholds.x)
        {
            //TODO: Send to peak, not falling!
            return BehaviourChangeRequest.New<PlayerFallingBehaviour>();
        }
        return null;
    }

    public override float CurrentGravity()
    {
        return PlayerController.PlayerStats.jumpGravity;
    }

    public override void Enter()
    {
        PlayerController.MovementController.SetVelocity(null, PlayerController.PlayerStats.jumpVelocity);
    }

    public override void Exit()
    {
    }

    public override void FixedUpdate(float delta)
    {
        base.FixedUpdate(delta);
    }

    public override void Update(float delta)
    {
        //throw new System.NotImplementedException();
    }
}