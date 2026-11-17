using System.Threading.Tasks;
using UnityEngine;
using Game.BehaviourMachine;
using Game.Player;
namespace Game.PlayerBehaviour
{
    public class PlayerLevelTransitionBehaviour : BasePlayerBehaviour
    {

        public override BehaviourChangeRequest VerifyBehaviour()
        {
            if (!BehaviourTarget.GameManager.ChangingLevel)
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

        public PlayerLevelTransitionBehaviour(PlayerController player) : base(player)
        {

        }

        public override void Enter()
        {
            BehaviourTarget.MovementController.enabled = false;
            BehaviourTarget.MovementController.SetVelocity(Vector2.zero);
            PlayAnim(BehaviourTarget.PlayerAnimator.AnimationList.LevelTransitionStart);
        }


        public override void Exit()
        {
            BehaviourTarget.MovementController.enabled = true;
            BehaviourTarget.PlaySFX(BehaviourTarget.SoundList.travelEndSFX);
            PlayAnim(BehaviourTarget.PlayerAnimator.AnimationList.LevelTransitionEnd);
        }

        public override void FixedUpdate(float delta)
        {
        }

        public override void Update(float delta)
        {
        }
    }

}