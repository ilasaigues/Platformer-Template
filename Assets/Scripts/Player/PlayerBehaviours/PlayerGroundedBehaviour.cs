using UnityEngine;
using Game.BehaviourMachine;
using Game.Player;

namespace Game.PlayerBehaviour
{
    public abstract class PlayerGroundedBehaviour : BasePlayerBehaviour
    {

        protected PlayerGroundedBehaviour(PlayerController player) : base(player)
        {
        }

        public override void Update(float delta)
        {
        }
        public override void FixedUpdate(float delta)
        {
            /*var gravity = PlayerController.PlayerStats.fallGravity;
            PlayerController.MovementController.AddVelocity(gravity * delta * Vector2.up);*/
        }

        public override BehaviourChangeRequest VerifyBehaviour()
        {
            if (base.VerifyBehaviour() is BehaviourChangeRequest baseBehaviour)
            {
                return baseBehaviour;
            }

            if (BehaviourTarget.InputHandler.JumpButton.JustPressed ||
                BehaviourTarget.InputHandler.JumpButton.TimeSinceLastPressed <=
                BehaviourTarget.PlayerStats.jumpBufferTime)
            {
                return BehaviourChangeRequest.New<PlayerJumpingBehaviour>();
            }
            if (!BehaviourTarget.MovementController.Grounded)
            {
                return BehaviourChangeRequest.New<PlayerFallingBehaviour>();
            }
            return null;
        }

        public override void Enter()
        {
            BehaviourTarget.ResetOnGrounded();
        }

    }
}