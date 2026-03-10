
using System;
using UnityEngine;


public class PlayerDashBehaviour : BasePlayerBehaviour, IPlayerAbilityBehaviour
{

    float _elapsedTime;

    int _direction;

    public bool OnCooldown => TimeLastUsed + PlayerController.AbilityStats.DashCooldown > Time.time;

    bool _windingUp = false;
    public PlayerDashBehaviour(PlayerController player) : base(player)
    {
    }


    public bool Enabled { get; set; }
    public float TimeLastUsed { get; set; }

    public override void Enter()
    {
        PlayAnim(PlayerController.PlayerAnimator.AnimationList.DashEnter);
        TimeLastUsed = Time.time;
        PlayerController.RemainingDashes--;

        _windingUp = true;
        _elapsedTime = 0;
        _direction = 0;
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
        /*else if (_elapsedTime <= PlayerController.AbilityStats.TotalDashTime) // winddown
        {
            PlayerController.MovementController.SetVelocity(Vector2.zero);
        }*/
    }

    public override void Update(float delta)
    {
    }

    public override void Exit()
    {
        EnqueueAnim(PlayerController.PlayerAnimator.AnimationList.DashExit);
        var flipX = PlayerController.MovementController.LastHorizontalDirection == -1;
        VFXSpawner.Instance.PlayFX(VFXSpawner.Instance.VFXList.DashFeathers, PlayerController.transform.position, flipX);
    }
}