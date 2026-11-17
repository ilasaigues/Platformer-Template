using UnityEngine;
using Game.BehaviourMachine;
using Game.Player;

namespace Game.PlayerBehaviour
{
    public class PlayerGroundMoveBehaviour : PlayerGroundedBehaviour
    {



        public PlayerGroundMoveBehaviour(PlayerController player) : base(player)
        {

        }
        public override void Enter()
        {
            base.Enter();
            EnqueueAnim(BehaviourTarget.PlayerAnimator.AnimationList.Run);
        }

        public override void Exit()
        {

        }


        public override void FixedUpdate(float delta)
        {
            base.FixedUpdate(delta);
            float currentVelocity = BehaviourTarget.MovementController.Velocity.x;
            int targetDirection = BehaviourTarget.LastDirectionInput.x.Sign0();
            float maxSpeed = BehaviourTarget.PlayerStats.groundedSpeed;

            float acceleration;
            if (targetDirection == 0 || currentVelocity.Sign0() * targetDirection == -1)
            {
                acceleration = maxSpeed / BehaviourTarget.PlayerStats.groundedDecelerationTime * delta;
            }
            else
            {
                acceleration = maxSpeed / BehaviourTarget.PlayerStats.groundedAccelerationTime * delta;
            }

            float difference = maxSpeed * targetDirection - currentVelocity;
            acceleration = Mathf.Clamp(acceleration, 0, Mathf.Abs(difference)) * Mathf.Sign(difference);

            BehaviourTarget.SetSpriteDirection(targetDirection);

            BehaviourTarget.MovementController.AddVelocity(acceleration * Vector2.right);

            /*var adjustedVelocity = PlayerController.CollisionController.CollideAndSlideVel(
                PlayerController.transform.position,
                delta * PlayerController.MovementController.Velocity.x * Vector2.right,
                LayerReference.TerrainLayer);
            PlayerController.MovementController.SetVelocity(adjustedVelocity.x / delta, null);*/

        }


        public override BehaviourChangeRequest VerifyBehaviour()
        {
            var baseValue = base.VerifyBehaviour();
            if (baseValue != null) return baseValue;
            if (BehaviourTarget.MovementController.Velocity.x == 0 && BehaviourTarget.LastDirectionInput.x == 0)
            {
                return BehaviourChangeRequest.New<PlayerIdleBehaviour>();
            }
            return null;
        }
    }
}