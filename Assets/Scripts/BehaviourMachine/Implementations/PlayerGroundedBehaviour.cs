using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerGroundedBehaviour : BasePlayerBehaviour
{
    protected float _lastHorizontalInput;


    protected PlayerGroundedBehaviour(PlayerController player) : base(player)
    {
    }

    public override void Update(float delta)
    {
        //_lastHorizontalInput = InputSystem.actions.getactio.GetAxis("Horizontal");
    }

    public override void Enter()
    {
        // check distance to ground vs collider extents on left and right side
        var playerLeft = PlayerController.transform.position;
        var playerRight = PlayerController.transform.position;
        playerLeft.x -= PlayerController.ColliderController.ColliderBounds.extents.x;
        playerRight.x += PlayerController.ColliderController.ColliderBounds.extents.x;
        var colliderMin = PlayerController.ColliderController.ColliderBounds.min;
        var hitLeft = PlayerController.CollisionController.CheckCollision(playerLeft, Vector2.down, 10, LayerReference.TerrainLayer);
        var hitRight = PlayerController.CollisionController.CheckCollision(playerRight, Vector2.down, 10, LayerReference.TerrainLayer);
        // calculate offset based on HIGHEST collision point (max)
        var offset = Vector2.up * (Mathf.Max(hitLeft.point.y, hitRight.point.y) - colliderMin.y);
        PlayerController.MovementController.ForceOffset(offset);
    }
}
