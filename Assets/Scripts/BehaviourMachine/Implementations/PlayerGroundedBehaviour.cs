using UnityEngine;

public abstract class PlayerGroundedBehaviour : BasePlayerBehaviour
{
    protected PlayerGroundedBehaviour(PlayerController player) : base(player)
    {
    }

    public override void Enter()
    {
        // check distance to ground vs collider extents
        var playerPos = PlayerController.transform.position;
        var colliderMin = PlayerController.ColliderController.ColliderBounds.min;
        var hit = PlayerController.CollisionController.CheckCollision(playerPos, Vector2.down, 10, LayerReference.TerrainLayer);
        var offset = Vector2.up * (hit.point.y - colliderMin.y);
        PlayerController.MovementController.ForceOffset(offset);
    }
}
