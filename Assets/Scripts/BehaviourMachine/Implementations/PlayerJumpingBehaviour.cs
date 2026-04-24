using UnityEngine;
public class PlayerJumpingBehaviour : PlayerAirBehaviour

{
    private bool _jumpHeld = false;
    private bool IsInPeak => PlayerController.MovementController.Velocity.y < PlayerController.PlayerStats.peakTresholds.x;
    public PlayerJumpingBehaviour(PlayerController playerController) : base(playerController)
    {
    }

    public override BehaviourChangeRequest VerifyBehaviour()
    {
        var newBehaviuourRequest = base.VerifyBehaviour();
        if (newBehaviuourRequest != null) return newBehaviuourRequest;


        if (!_jumpHeld && PlayerController.InputHandler.JumpButton.JustPressed && PlayerController.Jumps < 2)
        {
            return PlayerController.TryUseAbility<PlayerDoubleJumpBehaviour>(); ;
        }

        if (PlayerController.MovementController.Velocity.y < PlayerController.PlayerStats.peakTresholds.y ||
            (IsInPeak && !_jumpHeld))
        {

            return BehaviourChangeRequest.New<PlayerFallingBehaviour>();
        }
        return null;
    }

    public override float CurrentGravity()
    {
        if (IsInPeak)
        {
            return PlayerController.PlayerStats.peakGravity;
        }

        return _jumpHeld ? PlayerController.PlayerStats.jumpGravity : PlayerController.PlayerStats.cutoffGravity;
    }

    public override void Enter()
    {
        _jumpHeld = PlayerController.InputHandler.JumpButton.Pressed;
        PlayerController.MovementController.SetVelocity(null, PlayerController.PlayerStats.jumpVelocity);
        PlayerController.InputHandler.JumpButton.OnRelease += JumpReleased;
        PlayerController.Jumps = Mathf.Max(PlayerController.Jumps, 1);
        PlayAnim(PlayerController.PlayerAnimator.AnimationList.Jump, true);
        var flipX = PlayerController.MovementController.LastHorizontalDirection == -1;

        if (PlayerController.MovementController.Velocity.x != 0)
        {
            Vector2 spawnPosition = PlayerController.transform.position + new Vector3(
            -4.ToPixels() * PlayerController.MovementController.LastHorizontalDirection, 8.ToPixels(), 0);
            PlayerController.VFXSpawner.PlayFX(PlayerController.VFXSpawner.VFXList.JumpParticles_H, spawnPosition, 0, flipX);
        }
        else
        {
            Vector2 spawnPosition = PlayerController.transform.position + new Vector3(0, 8.ToPixels(), 0);
            PlayerController.VFXSpawner.PlayFX(PlayerController.VFXSpawner.VFXList.JumpParticles_V, spawnPosition, 0, flipX);
        }

    }

    private void JumpReleased()
    {
        _jumpHeld = false;
    }

    public override void Exit()
    {
        _jumpHeld = false;
        PlayerController.InputHandler.JumpButton.OnRelease -= JumpReleased;
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