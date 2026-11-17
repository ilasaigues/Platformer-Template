using UnityEngine;

using Game.Player;
using Game.BehaviourMachine;

namespace Game.PlayerBehaviour
{

    public class PlayerJumpingBehaviour : PlayerAirBehaviour

    {
        private bool _jumpHeld = false;
        private bool IsInPeak => BehaviourTarget.MovementController.Velocity.y < BehaviourTarget.PlayerStats.peakTresholds.x;
        public PlayerJumpingBehaviour(PlayerController playerController) : base(playerController)
        {
        }

        public override BehaviourChangeRequest VerifyBehaviour()
        {
            var newBehaviuourRequest = base.VerifyBehaviour();
            if (newBehaviuourRequest != null) return newBehaviuourRequest;


            if (!_jumpHeld && BehaviourTarget.InputHandler.JumpButton.JustPressed && BehaviourTarget.Jumps < 2)
            {
                return TryUseAbility<PlayerDoubleJumpBehaviour>(); ;
            }

            if (BehaviourTarget.MovementController.Velocity.y < BehaviourTarget.PlayerStats.peakTresholds.y ||
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
                return BehaviourTarget.PlayerStats.peakGravity;
            }

            return _jumpHeld ? BehaviourTarget.PlayerStats.jumpGravity : BehaviourTarget.PlayerStats.cutoffGravity;
        }

        public override void Enter()
        {
            BehaviourTarget.PlaySFX(BehaviourTarget.SoundList.jumpSFX);
            _jumpHeld = BehaviourTarget.InputHandler.JumpButton.Pressed;
            BehaviourTarget.MovementController.SetVelocity(null, BehaviourTarget.PlayerStats.jumpVelocity);
            BehaviourTarget.MovementController.Grounded = false;
            BehaviourTarget.InputHandler.JumpButton.OnRelease += JumpReleased;
            BehaviourTarget.Jumps = Mathf.Max(BehaviourTarget.Jumps, 1);
            PlayAnim(BehaviourTarget.PlayerAnimator.AnimationList.Jump, true);
            var flipX = BehaviourTarget.MovementController.LastHorizontalDirection == -1;

            if (BehaviourTarget.MovementController.Velocity.x != 0)
            {
                Vector2 spawnPosition = BehaviourTarget.transform.position + new Vector3(
                -4.ToPixels() * BehaviourTarget.MovementController.LastHorizontalDirection, 8.ToPixels(), 0);
                BehaviourTarget.VFXSpawner.PlayFX(BehaviourTarget.VFXSpawner.VFXList.JumpParticles_H, spawnPosition, 0, flipX);
            }
            else
            {
                Vector2 spawnPosition = BehaviourTarget.transform.position + new Vector3(0, 8.ToPixels(), 0);
                BehaviourTarget.VFXSpawner.PlayFX(BehaviourTarget.VFXSpawner.VFXList.JumpParticles_V, spawnPosition, 0, flipX);
            }

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
        }

        public override void Update(float delta)
        {
            //throw new System.NotImplementedException();
        }
    }
}