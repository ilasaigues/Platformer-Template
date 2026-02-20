using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerGroundedBehaviour : BasePlayerBehaviour
{

    protected PlayerGroundedBehaviour(PlayerController player) : base(player)
    {
    }

    public override void Update(float delta)
    {
    }

    public override BehaviourChangeRequest VerifyBehaviour()
    {

        if (PlayerController.CollisionController.GetClosestCollisonSurfaceDistance(
           Vector2.down,
           PlayerController.ColliderController.ColliderBounds,
           .1f,
           LayerReference.TerrainLayer) is null)
        {
            return BehaviourChangeRequest.New<PlayerFallingBehaviour>();
        }

        return null;
    }

    public override void Enter()
    {
        if (PlayerController.CollisionController.GetClosestCollisonSurfaceDistance(
            Vector2.down,
            PlayerController.ColliderController.ColliderBounds,
            .1f,
            LayerReference.TerrainLayer) is float offset)
        {
            PlayerController.MovementController.ForceOffset(offset * Vector2.down);
        }

    }

}
