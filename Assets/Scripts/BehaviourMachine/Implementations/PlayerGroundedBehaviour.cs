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
        // do a ground check, if no ground, go to falling

        return null;
    }

    public override void Enter()
    {
        if (PlayerController.CollisionController.GetClosestCollisonSurfaceDistance(
            Vector2.down,
            PlayerController.ColliderController.ColliderBounds,
            10,
            LayerReference.TerrainLayer) is float offset)
        {
            PlayerController.MovementController.ForceOffset(offset * Vector2.down);

        }

    }

}
