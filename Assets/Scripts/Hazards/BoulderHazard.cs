using System;
using UnityEngine;

[RequireComponent(typeof(ObjectMovementComponent))]
public class BoulderHazard : MonoBehaviour
{
    ObjectMovementComponent _movementComponent;

    public Vector2 Movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _movementComponent = GetComponent<ObjectMovementComponent>();
        _movementComponent.OnPlayerSqueezed += PlayerSqueezed;
    }

    private void PlayerSqueezed(PlayerController controller)
    {
        controller.MarkAsDead();
        controller.GainAbility<PlayerRockBehaviour>();
        _movementComponent.SetMovementControllerChild(null);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _movementComponent.SetVelocity(Movement);
    }
}
