using System;
using UnityEngine;

public class PlayerRockBehaviour : BasePlayerAbilityBehaviour
{


    public override bool OnCooldown => false;

    bool _released;

    public PlayerRockBehaviour(PlayerController player) : base(player)
    {

    }

    public override Type[] TransitionableBehaviourTypes => new Type[]
    {
        typeof(PlayerGroundedBehaviour),
        typeof(PlayerAirBehaviour),
    };


    public override void Enter()
    {
        base.Enter();
        _released = false;
        PlayerController.SpriteRenderer.color = Color.grey;
        PlayerController.InputHandler.RockButton.OnRelease += OnRockButtonRelease;
    }

    public override void Exit()
    {
        PlayerController.SpriteRenderer.color = Color.teal;
        PlayerController.InputHandler.RockButton.OnRelease -= OnRockButtonRelease;
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

        if (_released)
        {
            return BehaviourChangeRequest.New<PlayerFallingBehaviour>();
        }
        return null;
    }
}
