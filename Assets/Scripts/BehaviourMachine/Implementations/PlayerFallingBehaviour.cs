public class PlayerFallingBehaviour : PlayerAirBehaviour

{
    public PlayerFallingBehaviour(PlayerController playerController) : base(playerController)
    {
    }

    public override float CurrentGravity()
    {
        return PlayerController.PlayerStats.fallGravity;
    }

    public override void Enter()
    {
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