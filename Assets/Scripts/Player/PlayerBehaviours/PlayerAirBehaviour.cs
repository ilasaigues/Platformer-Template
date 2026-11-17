using UnityEngine;
using Game.BehaviourMachine;
using Game.Player;

namespace Game.PlayerBehaviour
{
    public abstract class PlayerAirBehaviour : BasePlayerBehaviour
    {
        public PlayerAirBehaviour(PlayerController playerController) : base(playerController) { }
        public override BehaviourChangeRequest VerifyBehaviour()
        {
            if (base.VerifyBehaviour() is BehaviourChangeRequest baseBehaviour)
            {
                return baseBehaviour;
            }

            var verticalVelocity = BehaviourTarget.MovementController.Velocity.y;

            if (verticalVelocity <= 0 &&
                BehaviourTarget.MovementController.Grounded)
            {
                PlayAnim(BehaviourTarget.PlayerAnimator.AnimationList.Land);

                if (Mathf.Approximately(BehaviourTarget.LastDirectionInput.x, 0))
                {
                    return BehaviourChangeRequest.New<PlayerIdleBehaviour>();
                }
                else
                {
                    return BehaviourChangeRequest.New<PlayerGroundMoveBehaviour>();
                }
            }

            return null;
        }

        public override void FixedUpdate(float delta)
        {
            var gravity = CurrentGravity();
            BehaviourTarget.MovementController.AddVelocity(gravity * delta * Vector2.up);

            float currentVelocity = BehaviourTarget.MovementController.Velocity.x;
            int targetDirection = BehaviourTarget.LastDirectionInput.x.Sign0();
            float maxSpeed = BehaviourTarget.PlayerStats.airSpeed;

            float acceleration;
            if (targetDirection == 0 || currentVelocity.Sign0() * targetDirection == -1)
            {
                acceleration = maxSpeed / BehaviourTarget.PlayerStats.airDecelerationTime * delta;
            }
            else
            {
                acceleration = maxSpeed / BehaviourTarget.PlayerStats.airAccelerationTime * delta;
            }

            float difference = maxSpeed * targetDirection - currentVelocity;
            acceleration = Mathf.Clamp(acceleration, 0, Mathf.Abs(difference)) * Mathf.Sign(difference);

            BehaviourTarget.SetSpriteDirection(targetDirection);
            BehaviourTarget.MovementController.AddVelocity(acceleration * Vector2.right);
        }

        public abstract float CurrentGravity();
    }

}