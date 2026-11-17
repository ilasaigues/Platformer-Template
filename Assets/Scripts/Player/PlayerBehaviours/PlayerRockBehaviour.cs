using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Player;
using Game.BehaviourMachine;

namespace Game.PlayerBehaviour
{
    public class PlayerRockBehaviour : BasePlayerBehaviour, IPlayerAbilityBehaviour
    {
        public AnimationClip UIAnimation => BehaviourTarget.AbilityStats.ShieldUIAnimation;


        public bool Enabled { get; set; }
        public bool OnCooldown => false;

        public float TimeLastUsed { get; set; }

        bool _released;

        public PlayerRockBehaviour(PlayerController player) : base(player)
        {

        }


        public override void Enter()
        {
            _released = false;
            BehaviourTarget.PlaySFX(BehaviourTarget.SoundList.shieldEnterSFX);
            TimeLastUsed = BehaviourTarget.TimeContext.Time;
            BehaviourTarget.InputHandler.RockButton.OnRelease += OnRockButtonRelease;
            BehaviourTarget.MovementController.CanBeSqueezed = false;
            PlayAnim(BehaviourTarget.PlayerAnimator.AnimationList.ShieldEnter);
            BehaviourTarget.MovementController.VerticalTerminalVelocity = BehaviourTarget.AbilityStats.RockTerminalVelocity;
        }

        public override void Exit()
        {
            BehaviourTarget.InputHandler.RockButton.OnRelease -= OnRockButtonRelease;
            var flipX = BehaviourTarget.MovementController.LastHorizontalDirection == -1;
            BehaviourTarget.MovementController.CanBeSqueezed = true;
            if (BehaviourTarget.MovementController.Grounded)
            {
                BehaviourTarget.MovementController.SetVelocity(Vector2.zero);
            }
            else
            {
                BehaviourTarget.MovementController.SetVelocity(BehaviourTarget.MovementController.Velocity.x * 0.5f, 1);
            }
            BehaviourTarget.VFXSpawner.PlayFX(BehaviourTarget.VFXSpawner.VFXList.ShieldEnd, BehaviourTarget.transform.position + Vector3.up * 4 / 16f, -2, false);
            BehaviourTarget.MovementController.VerticalTerminalVelocity = BehaviourTarget.PlayerStats.fallVelocityCap;

        }

        private void OnRockButtonRelease()
        {
            _released = true;
        }

        public override void FixedUpdate(float delta)
        {
            var gravity = BehaviourTarget.AbilityStats.RockGravity;
            BehaviourTarget.MovementController.AddVelocity(gravity * delta * Vector2.up);

            var playerVelocity = BehaviourTarget.MovementController.Velocity;

            var horizontalThreshold = Mathf.Abs(playerVelocity.x) >= BehaviourTarget.AbilityStats.RockBreakSpeeds.x;
            var verticalThreshold = Mathf.Abs(playerVelocity.y) >= BehaviourTarget.AbilityStats.RockBreakSpeeds.y;

            var playerCollider = BehaviourTarget.CollisionController.MainCollider;

            var hits = new List<RaycastHit2D>();
            if (horizontalThreshold)
            {
                hits.AddRange(BoxCaster2D.GetHits(playerCollider.bounds.center, playerCollider.bounds, playerVelocity.x * BehaviourTarget.TimeContext.FixedDeltaTime * Vector2.right, LayerReference.TerrainLayer, 1));
            }

            if (verticalThreshold)
            {
                hits.AddRange(BoxCaster2D.GetHits(playerCollider.bounds.center, playerCollider.bounds, playerVelocity.y * BehaviourTarget.TimeContext.FixedDeltaTime * Vector2.up, LayerReference.TerrainLayer, 1));
            }
            if (verticalThreshold || horizontalThreshold)
            {
                foreach (var breakable in hits.Select(hit => hit.collider.GetComponent<BreakableTerrainBehaviour>()).Where(btb => btb != null))
                {
                    breakable.Break();
                }
            }

            if (BehaviourTarget.MovementController.Grounded)
            {
                var maxSpeed = BehaviourTarget.PlayerStats.groundedSpeed;
                var currentVelocity = BehaviourTarget.MovementController.Velocity.x;
                var acceleration = maxSpeed / BehaviourTarget.PlayerStats.groundedDecelerationTime * delta * -currentVelocity.Sign0();

                if (Mathf.Abs(acceleration) > Mathf.Abs(currentVelocity))
                {
                    acceleration = -currentVelocity;
                }

                BehaviourTarget.MovementController.AddVelocity(acceleration * Vector2.right);
            }
        }

        public override void Update(float delta)
        {
            //throw new System.NotImplementedException();
        }

        public override BehaviourChangeRequest VerifyBehaviour()
        {
            if (base.VerifyBehaviour() is BehaviourChangeRequest baseBehaviour)
            {
                return baseBehaviour;
            }

            if (_released && BehaviourTarget.TimeContext.Time - TimeLastUsed >= BehaviourTarget.AbilityStats.MinRockTime)
            {
                BehaviourTarget.PlaySFX(BehaviourTarget.SoundList.shieldExitSFX);
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
    }
}