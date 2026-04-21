using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerRockBehaviour : BasePlayerBehaviour, IPlayerAbilityBehaviour
{


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
        TimeLastUsed = PlayerController.TimeContext.Time;
        PlayerController.InputHandler.RockButton.OnRelease += OnRockButtonRelease;
        PlayerController.MovementController.CanBeSqueezed = false;
        PlayAnim(PlayerController.PlayerAnimator.AnimationList.ShieldEnter);
        PlayerController.MovementController.VerticalTerminalVelocity = PlayerController.AbilityStats.RockTerminalVelocity;
    }

    public override void Exit()
    {
        PlayerController.InputHandler.RockButton.OnRelease -= OnRockButtonRelease;
        var flipX = PlayerController.MovementController.LastHorizontalDirection == -1;
        PlayerController.MovementController.CanBeSqueezed = true;
        if (PlayerController.MovementController.Grounded)
        {
            PlayerController.MovementController.SetVelocity(Vector2.zero);
        }
        else
        {
            PlayerController.MovementController.SetVelocity(PlayerController.MovementController.Velocity.x * 0.5f, 1);
        }
        VFXSpawner.Instance.PlayFX(VFXSpawner.Instance.VFXList.ShieldEnd, PlayerController.transform.position + Vector3.up * 4 / 16f, -2, false);
        PlayerController.MovementController.VerticalTerminalVelocity = PlayerController.PlayerStats.fallVelocityCap;

    }

    private void OnRockButtonRelease()
    {
        _released = true;
    }

    public override void FixedUpdate(float delta)
    {
        var gravity = PlayerController.AbilityStats.RockGravity;
        PlayerController.MovementController.AddVelocity(gravity * delta * Vector2.up);

        var playerVelocity = PlayerController.MovementController.Velocity;

        var horizontalThreshold = Mathf.Abs(playerVelocity.x) >= PlayerController.AbilityStats.RockBreakSpeeds.x;
        var verticalThreshold = Mathf.Abs(playerVelocity.y) >= PlayerController.AbilityStats.RockBreakSpeeds.y;

        var playerCollider = PlayerController.CollisionController.MainCollider;

        var hits = new List<RaycastHit2D>();
        if (horizontalThreshold)
        {
            hits.AddRange(BoxCaster2D.GetHits(playerCollider.bounds.center, playerCollider.bounds, playerVelocity.x * PlayerController.TimeContext.FixedDeltaTime * Vector2.right, LayerReference.TerrainLayer, 1));
        }

        if (verticalThreshold)
        {
            hits.AddRange(BoxCaster2D.GetHits(playerCollider.bounds.center, playerCollider.bounds, playerVelocity.y * PlayerController.TimeContext.FixedDeltaTime * Vector2.up, LayerReference.TerrainLayer, 1));
        }
        if (verticalThreshold || horizontalThreshold)
        {
            foreach (var breakable in hits.Select(hit => hit.collider.GetComponent<BreakableTerrainBehaviour>()).Where(btb => btb != null))
            {
                breakable.Break();
            }
        }

        if (PlayerController.MovementController.Grounded)
        {
            var maxSpeed = PlayerController.PlayerStats.groundedSpeed;
            var currentVelocity = PlayerController.MovementController.Velocity.x;
            var acceleration = maxSpeed / PlayerController.PlayerStats.groundedDecelerationTime * delta * -currentVelocity.Sign0();

            if (Mathf.Abs(acceleration) > Mathf.Abs(currentVelocity))
            {
                acceleration = -currentVelocity;
            }

            PlayerController.MovementController.AddVelocity(acceleration * Vector2.right);
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

        if (_released && PlayerController.TimeContext.Time - TimeLastUsed >= PlayerController.AbilityStats.MinRockTime)
        {
            if (PlayerController.MovementController.Grounded)
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
