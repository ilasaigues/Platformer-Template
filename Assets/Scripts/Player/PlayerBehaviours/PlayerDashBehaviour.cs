
using System;
using UnityEditor;
using UnityEngine;
using Game.BehaviourMachine;
using Game.Player;

namespace Game.PlayerBehaviour
{

    public class PlayerDashBehaviour : BasePlayerBehaviour, IPlayerAbilityBehaviour
    {

        public AnimationClip UIAnimation => BehaviourTarget.AbilityStats.DashUIAnimation;
        float _elapsedTime;

        int _direction;

        public bool OnCooldown => TimeLastUsed + BehaviourTarget.AbilityStats.DashCooldown > BehaviourTarget.TimeContext.Time;

        bool _windingUp = false;
        public PlayerDashBehaviour(PlayerController player) : base(player)
        {
        }


        public bool Enabled { get; set; }
        public float TimeLastUsed { get; set; }

        public override void Enter()
        {
#if UNITY_EDITOR
            if (BehaviourTarget.AbilityStats.DashPause) EditorApplication.isPaused = true;
#endif
            PlayAnim(BehaviourTarget.PlayerAnimator.AnimationList.DashEnter);
            TimeLastUsed = BehaviourTarget.TimeContext.Time;
            BehaviourTarget.RemainingDashes--;

            _windingUp = true;
            _elapsedTime = 0;

            _direction = BehaviourTarget.FacingDirection;
            BehaviourTarget.CollisionController.ResizeMainCollider(BehaviourTarget.PlayerStats.DashColliderSize, Vector2.zero);
            BehaviourTarget.MovementController.CheckAndFixOverlap();
        }

        public override BehaviourChangeRequest VerifyBehaviour()
        {
            if (base.VerifyBehaviour() is BehaviourChangeRequest baseBehaviour)
            {
                return baseBehaviour;
            }

            if (_elapsedTime > BehaviourTarget.AbilityStats.TotalDashTime)
            {
                if (BehaviourTarget.MovementController.Grounded)
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
            if (_elapsedTime <= BehaviourTarget.AbilityStats.DashWindupTime) // windup
            {
                //  decelerate or break all speeds
                BehaviourTarget.MovementController.SetVelocity(Vector2.zero);
            }
            else if (_elapsedTime <= BehaviourTarget.AbilityStats.DashWindupTime + BehaviourTarget.AbilityStats.DashMovementTime) // dash
            {
                if (_windingUp)
                {
                    PlayAnim(BehaviourTarget.PlayerAnimator.AnimationList.Dash);
                    BehaviourTarget.StartTrail();
                    _windingUp = false;
                    BehaviourTarget.PlaySFX(BehaviourTarget.SoundList.dashSFX);
                }
                if (_direction == 0)
                {
                    _direction = BehaviourTarget.LastDirectionInput.x.Sign0();
                    BehaviourTarget.SetSpriteDirection(_direction);
                    if (_direction == 0)
                    {
                        _direction = BehaviourTarget.MovementController.LastHorizontalDirection;
                    }
                    if (_direction == 0)
                    {
                        _direction = 1;
                    }
                }
                //  move constantly in direction by speed (dashDistance/dashTime)
                BehaviourTarget.MovementController.SetVelocity(BehaviourTarget.AbilityStats.DashVelocity * _direction, null);
            }
            else if (_elapsedTime <= BehaviourTarget.AbilityStats.TotalDashTime) // winddown
            {
                BehaviourTarget.CollisionController.ResizeMainCollider(BehaviourTarget.PlayerStats.DefaultColliderSize, Vector2.zero);
                BehaviourTarget.MovementController.CheckAndFixOverlap();
                BehaviourTarget.MovementController.SetVelocity(BehaviourTarget.AbilityStats.DashVelocity * BehaviourTarget.AbilityStats.DashEndMultiplier * _direction, null);
            }
        }

        public override void Update(float delta)
        {
        }

        public override void Exit()
        {
            EnqueueAnim(BehaviourTarget.PlayerAnimator.AnimationList.DashExit);
            BehaviourTarget.ToggleDashParticles(0);
        }
    }
}