using System;
using UnityEngine;
using Game.Player;
using Game.BehaviourMachine;

namespace Game.PlayerBehaviour
{

    public class PlayerFallingBehaviour : PlayerAirBehaviour
    {
        public PlayerFallingBehaviour(PlayerController playerController) : base(playerController)
        {
        }

        public override float CurrentGravity()
        {
            return BehaviourTarget.PlayerStats.fallGravity;
        }

        public override void Enter()
        {
            EnqueueAnim(BehaviourTarget.PlayerAnimator.AnimationList.Peak);
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
        }

        public override BehaviourChangeRequest VerifyBehaviour()
        {
            var baseValue = base.VerifyBehaviour();
            if (baseValue != null) return baseValue;


            bool jumpRequested = BehaviourTarget.InputHandler.JumpButton.JustPressed;
            if (jumpRequested)
            {
                bool isInCoyoteTime = BehaviourTarget.TimeContext.Time - BehaviourTarget.MovementController.TimeLeftGround <=
                   BehaviourTarget.PlayerStats.coyoteTime;
                if (isInCoyoteTime && BehaviourTarget.Jumps < 1)
                {
                    return BehaviourChangeRequest.New<PlayerJumpingBehaviour>();
                }
                else if (BehaviourTarget.Jumps < 2)
                {

                    return TryUseAbility<PlayerDoubleJumpBehaviour>(); ;
                }
            }
            return null;
        }
    }
}