
using System;
using UnityEngine;


public class PlayerDashBehaviour : BasePlayerAbilityBehaviour
{

    float _elapsedTime;

    int _direction;

    public override bool OnCooldown => _timeLastUsed + TimeSpan.FromSeconds(PlayerController.AbilityStats.DashCooldown) > DateTime.Now;

    public PlayerDashBehaviour(PlayerController player) : base(player)
    {
    }

    public override Type[] TransitionableBehaviourTypes => new Type[]
    {
        typeof(PlayerGroundedBehaviour),
        typeof(PlayerAirBehaviour),
    };

    public override void Enter()
    {
        base.Enter();
        PlayerController.SpriteRenderer.color = Color.orange;
        PlayerController.RemainingDashes--;

        _elapsedTime = 0;
        _direction = 0;
    }

    public override void Exit()
    {
        PlayerController.SpriteRenderer.color = Color.teal;
    }

    public override BehaviourChangeRequest VerifyBehaviour()
    {
        if (base.VerifyBehaviour() is BehaviourChangeRequest baseBehaviour)
        {
            return baseBehaviour;
        }

        if (_elapsedTime > PlayerController.AbilityStats.TotalDashTime)
        {
            return BehaviourChangeRequest.New<PlayerFallingBehaviour>();
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
            if (_direction == 0)
            {
                _direction = PlayerController.LastDirectionInput.x.Sign0();
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
}