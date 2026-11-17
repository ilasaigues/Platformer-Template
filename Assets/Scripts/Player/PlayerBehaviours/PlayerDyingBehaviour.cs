using System.Diagnostics;
using System.Threading;
using UnityEngine;
using Game.BehaviourMachine;
using Game.Player;

namespace Game.PlayerBehaviour
{
    public class PlayerDyingBehaviour : BasePlayerBehaviour
    {

        private float _timeRemaining;

        private bool _isDying;

        private bool _startedMovementTransition;

        public PlayerDyingBehaviour(PlayerController player) : base(player)
        {
        }

        public override void Enter()
        {
            _isDying = true;
            _startedMovementTransition = false;
            _timeRemaining = BehaviourTarget.PlayerStats.TotalDeathTime;
            BehaviourTarget.CollisionController.MainCollider.enabled = false;
            PlayAnim(BehaviourTarget.PlayerAnimator.AnimationList.Death);
            BehaviourTarget.MovementController.SetVelocity(Vector2.zero);

            var respawnPos = BehaviourTarget.GameManager.GetRespawnPosition();
            Vector2 particlePosition = respawnPos + Vector3.up * 8.ToPixels();
            BehaviourTarget.VFXSpawner.PlayFX(BehaviourTarget.VFXSpawner.VFXList.Respawn_Particles, particlePosition, 1, false);

        }

        public override void Exit()
        {
            BehaviourTarget.CollisionController.MainCollider.enabled = true;
        }

        public override void FixedUpdate(float delta)
        {
            // no op
        }

        public override void Update(float delta)
        {
            // change to respawn position (and stick to ground) and animation after death duration


            if (!_startedMovementTransition)
            {
                _startedMovementTransition = true;
                LeanTween.move(
                    BehaviourTarget.gameObject,
                    BehaviourTarget.GameManager.GetRespawnPosition(),
                    BehaviourTarget.PlayerStats.DeathDuration * .5f)
                    .setEaseOutQuad().setDelay(BehaviourTarget.PlayerStats.DeathDuration * .75f)
                    .setTimeContext(BehaviourTarget.TimeContext);
            }


            if (_isDying && _timeRemaining <= BehaviourTarget.PlayerStats.ReviveDuration)
            {
                _isDying = false;
                BehaviourTarget.Respawn();

                PlayAnim(BehaviourTarget.PlayerAnimator.AnimationList.Revive);
            }

            _timeRemaining -= delta;
        }

        public override BehaviourChangeRequest VerifyBehaviour()
        {
            if (base.VerifyBehaviour() is BehaviourChangeRequest baseRequest)
            {
                return baseRequest;
            }

            if (_timeRemaining <= 0)
            {
                return BehaviourChangeRequest.New<PlayerJumpingBehaviour>();
            }

            return null;
        }
    }
}