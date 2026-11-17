using System;
using UnityEngine;
using Game.BehaviourMachine;
using Game.Player;

namespace Game.PlayerBehaviour
{
    public class PlayerDoubleJumpBehaviour : PlayerAirBehaviour, IPlayerAbilityBehaviour

    {
        public AnimationClip UIAnimation => BehaviourTarget.AbilityStats.DoubleJumpUIAnimation;

        private bool _jumpHeld = false;
        private bool IsInPeak => BehaviourTarget.MovementController.Velocity.y < BehaviourTarget.PlayerStats.peakTresholds.x;

        public bool Enabled { get; set; }

        public bool OnCooldown => false;

        public float TimeLastUsed { get; set; }

        private bool IsInWindup = false;
        public PlayerDoubleJumpBehaviour(PlayerController playerController) : base(playerController)
        {
        }

        public override BehaviourChangeRequest VerifyBehaviour()
        {
            var newBehaviuourRequest = base.VerifyBehaviour();
            if (newBehaviuourRequest != null) return newBehaviuourRequest;

            if (!IsInWindup && (BehaviourTarget.MovementController.Velocity.y < BehaviourTarget.PlayerStats.peakTresholds.y ||
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
                return BehaviourTarget.PlayerStats.peakGravity;
            }

            return !_jumpHeld && BehaviourTarget.TimeContext.Time - TimeLastUsed >=
                BehaviourTarget.AbilityStats.DoubleJumpMinTime + BehaviourTarget.AbilityStats.DoubleJumpWindupTime
                ? BehaviourTarget.PlayerStats.cutoffGravity : BehaviourTarget.PlayerStats.jumpGravity;
        }

        public override void Enter()
        {
            TimeLastUsed = BehaviourTarget.TimeContext.Time;
            _jumpHeld = BehaviourTarget.InputHandler.JumpButton.Pressed;
            IsInWindup = true;
            BehaviourTarget.MovementController.SetVelocity(null, BehaviourTarget.AbilityStats.DoubleJumpHoverVelocity);
            BehaviourTarget.InputHandler.JumpButton.OnRelease += JumpReleased;
            BehaviourTarget.Jumps = Mathf.Max(BehaviourTarget.Jumps, 2);
            PlayAnim(BehaviourTarget.PlayerAnimator.AnimationList.DoubleJump);

        }

        private void JumpReleased()
        {
            _jumpHeld = false;
        }

        public override void Exit()
        {
            _jumpHeld = false;
            BehaviourTarget.InputHandler.JumpButton.OnRelease -= JumpReleased;
        }

        public override void FixedUpdate(float delta)
        {
            base.FixedUpdate(delta);
            if (IsInWindup)
            {
                if (BehaviourTarget.TimeContext.Time >= TimeLastUsed + BehaviourTarget.AbilityStats.DoubleJumpWindupTime)
                {
                    IsInWindup = false;
                    BehaviourTarget.MovementController.SetVelocity(null, BehaviourTarget.AbilityStats.DoubleJumpVelocity);
                    var flipX = BehaviourTarget.MovementController.LastHorizontalDirection == -1;
                    //VFXSpawner.Instance.PlayFX(VFXSpawner.Instance.VFXList.AirBurst, PlayerController.transform.position, flipX);
                }
            }
        }

        public override void Update(float delta)
        {
            //throw new System.NotImplementedException();
        }
    }
}