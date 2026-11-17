using UnityEngine;
using Game.BehaviourMachine;
using Game.Player;

namespace Game.PlayerBehaviour
{
    public class PlayerIdleBehaviour : PlayerGroundedBehaviour
    {
        public PlayerIdleBehaviour(PlayerController player) : base(player)
        {

        }

        public override void Enter()
        {
            base.Enter();
            EnqueueAnim(BehaviourTarget.PlayerAnimator.AnimationList.Idle);
        }

        public override void Exit()
        {
            //throw new System.NotImplementedException();
        }

        public override void Update(float delta)
        {

        }

        public override void FixedUpdate(float delta)
        {
            base.FixedUpdate(delta);

            float currentVelocity = BehaviourTarget.MovementController.Velocity.x;
            int targetDirection = BehaviourTarget.LastDirectionInput.x.Sign0();
            float maxSpeed = BehaviourTarget.PlayerStats.groundedSpeed;

            float acceleration = maxSpeed / BehaviourTarget.PlayerStats.groundedDecelerationTime * delta;

            float difference = maxSpeed * targetDirection - currentVelocity;
            acceleration = Mathf.Clamp(acceleration, 0, Mathf.Abs(difference)) * Mathf.Sign(difference);

            BehaviourTarget.SetSpriteDirection(targetDirection);

            BehaviourTarget.MovementController.AddVelocity(acceleration * Vector2.right);
        }


        public override BehaviourChangeRequest VerifyBehaviour()
        {
            var baseValue = base.VerifyBehaviour();
            if (baseValue != null) return baseValue;
            if (BehaviourTarget.LastDirectionInput.x != 0)
            {
                return BehaviourChangeRequest.New<PlayerGroundMoveBehaviour>();
            }

            return null;
        }

    }
}