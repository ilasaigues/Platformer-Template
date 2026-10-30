
using System;
using UnityEditor;
using UnityEngine;


public class PlayerDashBehaviour : BasePlayerBehaviour, IPlayerAbilityBehaviour
{

    public AnimationClip UIAnimation => PlayerController.AbilityStats.DashUIAnimation;
    float _elapsedTime;

    int _direction;

    public bool OnCooldown => TimeLastUsed + PlayerController.AbilityStats.DashCooldown > PlayerController.TimeContext.Time;

    bool _windingUp = false;
    public PlayerDashBehaviour(PlayerController player) : base(player)
    {
    }


    public bool Enabled { get; set; }
    public float TimeLastUsed { get; set; }

    public override void Enter()
    {
        #if UNITY_EDITOR
        if (PlayerController.AbilityStats.DashPause) EditorApplication.isPaused = true;
        #endif
        PlayAnim(PlayerController.PlayerAnimator.AnimationList.DashEnter);
        TimeLastUsed = PlayerController.TimeContext.Time;
        PlayerController.RemainingDashes--;

        _windingUp = true;
        _elapsedTime = 0;

        _direction = PlayerController.FacingDirection;
        PlayerController.CollisionController.ResizeMainCollider(PlayerController.PlayerStats.DashColliderSize, Vector2.zero);
        PlayerController.MovementController.CheckAndFixOverlap();
    }

    public override BehaviourChangeRequest VerifyBehaviour()
    {
        if (base.VerifyBehaviour() is BehaviourChangeRequest baseBehaviour)
        {
            return baseBehaviour;
        }

        if (_elapsedTime > PlayerController.AbilityStats.TotalDashTime)
        {
            if (PlayerController.MovementController.Grounded)
            {
                return BehaviourChangeRequest.New<PlayerIdleBehaviour>();
            }
            else
            {
                return BehaviourChangeRequest.New<PlayerFallingBehaviour>();
            }
        }
        return null;
    }

    public override void FixedUpdate(float delta)
    {
        _elapsedTime += delta;
        if (_elapsedTime <= PlayerController.AbilityStats.DashWindupTime) // windup
        {
            //  decelerate or break all speeds
            PlayerController.MovementController.SetVelocity(Vector2.zero);
        }
        else if (_elapsedTime <= PlayerController.AbilityStats.DashWindupTime + PlayerController.AbilityStats.DashMovementTime) // dash
        {
            if (_windingUp)
            {
                PlayAnim(PlayerController.PlayerAnimator.AnimationList.Dash);
                PlayerController.StartTrail();
                _windingUp = false;
                PlayerController.PlaySFX(PlayerController.SoundList.dashSFX);
            }
            if (_direction == 0)
            {
                _direction = PlayerController.LastDirectionInput.x.Sign0();
                PlayerController.SetSpriteDirection(_direction);
                if (_direction == 0)
                {
                    _direction = PlayerController.MovementController.LastHorizontalDirection;
                }
                if (_direction == 0)
                {
                    _direction = 1;
                }
            }
            //  move constantly in direction by speed (dashDistance/dashTime)
            PlayerController.MovementController.SetVelocity(PlayerController.AbilityStats.DashVelocity * _direction, null);
        }
        else if (_elapsedTime <= PlayerController.AbilityStats.TotalDashTime) // winddown
        {
            PlayerController.CollisionController.ResizeMainCollider(PlayerController.PlayerStats.DefaultColliderSize, Vector2.zero);
            PlayerController.MovementController.CheckAndFixOverlap();
            PlayerController.MovementController.SetVelocity(PlayerController.AbilityStats.DashVelocity * PlayerController.AbilityStats.DashEndMultiplier * _direction, null);
        }
    }

    public override void Update(float delta)
    {
    }

    public override void Exit()
    {
        EnqueueAnim(PlayerController.PlayerAnimator.AnimationList.DashExit);
        PlayerController.ToggleDashParticles(0);
    }
}