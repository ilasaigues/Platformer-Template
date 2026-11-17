using UnityEngine;
using Game.Player;
using Game.BehaviourMachine;

namespace Game.PlayerBehaviour
{
    public abstract class BasePlayerBehaviour : BaseBehaviour<PlayerController>
    {
        public BasePlayerBehaviour(PlayerController target) : base(target) { }

        public override BehaviourChangeRequest VerifyBehaviour()
        {
            if (BehaviourTarget.GameManager.ChangingLevel)
            {
                if (this is PlayerLevelTransitionBehaviour) return null;
                return BehaviourChangeRequest.New<PlayerLevelTransitionBehaviour>();
            }

            if (BehaviourTarget.IsDead)
            {
                return BehaviourChangeRequest.New<PlayerDyingBehaviour>();
            }

            if (BehaviourTarget.InputHandler.RockButton.JustPressed)
            {
                return TryUseAbility<PlayerRockBehaviour>();
            }
            if (BehaviourTarget.InputHandler.DashButton.JustPressed && BehaviourTarget.TryDash())
            {
                return TryUseAbility<PlayerDashBehaviour>();
            }
            return null;
        }

        public void PlayAnim(AnimationClip animClip, bool clearQueue = true)
        {
            BehaviourTarget.PlayerAnimator.PlayAnimationClip(animClip, clearQueue);
        }

        public void EnqueueAnim(AnimationClip animClip)
        {
            BehaviourTarget.PlayerAnimator.EnqueueAnimationClip(animClip);
        }

        public BehaviourChangeRequest TryUseAbility<T>() where T : BasePlayerBehaviour, IPlayerAbilityBehaviour
        {
            var ability = BehaviourTarget.BehaviourMachine.GetBehaviour<T>();
            if (ability != null && ability.Enabled && !ability.OnCooldown)
            {
                return BehaviourChangeRequest.New<T>();
            }
            return null;
        }

    }
}