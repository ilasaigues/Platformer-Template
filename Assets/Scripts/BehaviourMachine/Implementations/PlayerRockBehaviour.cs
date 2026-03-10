using System;
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
        TimeLastUsed = Time.time;
        PlayerController.InputHandler.RockButton.OnRelease += OnRockButtonRelease;
        PlayAnim(PlayerController.PlayerAnimator.AnimationList.ShieldEnter);
    }

    public override void Exit()
    {
        PlayerController.InputHandler.RockButton.OnRelease -= OnRockButtonRelease;
        var flipX = PlayerController.MovementController.LastHorizontalDirection == -1;
        VFXSpawner.Instance.PlayFX(VFXSpawner.Instance.VFXList.ShieldEnd, PlayerController.transform.position, flipX);
    }

    private void OnRockButtonRelease()
    {
        _released = true;
    }

    public override void FixedUpdate(float delta)
    {
        var gravity = PlayerController.AbilityStats.RockGravity;
        PlayerController.MovementController.AddVelocity(gravity * delta * Vector2.up);

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

        if (_released && Time.time - TimeLastUsed >= PlayerController.AbilityStats.MinRockTime)
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
