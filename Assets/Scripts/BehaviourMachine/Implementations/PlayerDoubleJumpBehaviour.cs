using System;
using UnityEngine;

public class PlayerDoubleJumpBehaviour : PlayerAirBehaviour, IPlayerAbilityBehaviour

{
    private bool _jumpHeld = false;
    private bool IsInPeak => PlayerController.MovementController.Velocity.y < PlayerController.PlayerStats.peakTresholds.x;

    public bool Enabled { get; set; }

    public bool OnCooldown => false;

    public DateTime TimeLastUsed { get; set; }

    private bool IsInWindup = false;
    public PlayerDoubleJumpBehaviour(PlayerController playerController) : base(playerController)
    {
    }

    public override BehaviourChangeRequest VerifyBehaviour()
    {
        var newBehaviuourRequest = base.VerifyBehaviour();
        if (newBehaviuourRequest != null) return newBehaviuourRequest;

        if (!IsInWindup && (PlayerController.MovementController.Velocity.y < PlayerController.PlayerStats.peakTresholds.y ||
            (IsInPeak && !_jumpHeld)))
        {
            return BehaviourChangeRequest.New<PlayerFallingBehaviour>();
        }

        return null;
    }

    public override float CurrentGravity()
    {
        if (IsInWindup)
        {
            return 0;
        }
        if (IsInPeak)
        {
            return PlayerController.PlayerStats.peakGravity;
        }

        return _jumpHeld ? PlayerController.PlayerStats.jumpGravity : PlayerController.PlayerStats.cutoffGravity;
    }

    public override void Enter()
    {
        TimeLastUsed = DateTime.Now;
        _jumpHeld = PlayerController.InputHandler.JumpButton.Pressed;
        IsInWindup = true;
        PlayerController.MovementController.SetVelocity(null, PlayerController.AbilityStats.DoubleJumpHoverVelocity);
        PlayerController.InputHandler.JumpButton.OnRelease += JumpReleased;
        PlayerController.Jumps = Mathf.Max(PlayerController.Jumps, 2);
        PlayAnim(PlayerController.PlayerAnimator.AnimationList.DoubleJump);

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
        if (IsInWindup)
        {
            if (DateTime.Now >= TimeLastUsed + TimeSpan.FromSeconds(PlayerController.AbilityStats.DoubleJumpWindupTime))
            {
                IsInWindup = false;
                PlayerController.MovementController.SetVelocity(null, PlayerController.AbilityStats.DoubleJumpVelocity);
            }
        }
    }

    public override void Update(float delta)
    {
        //throw new System.NotImplementedException();
    }
}