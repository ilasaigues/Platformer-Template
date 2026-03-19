using System;
using UnityEngine;

[RequireComponent(typeof(ObjectMovementComponent))]
public class BoulderHazard : MonoBehaviour
{
    public Vector2[] Velocities;
    private int _currentVelocityIndex = 0;
    ObjectMovementComponent _movementComponent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _movementComponent = GetComponent<ObjectMovementComponent>();
        _movementComponent.OnObstacleHit += ObstacleHit;
    }

    private void ObstacleHit()
    {
        _currentVelocityIndex = (_currentVelocityIndex + 1) % Velocities.Length;
        _movementComponent.SetVelocity(Velocities[_currentVelocityIndex]);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _movementComponent.SetVelocity(Velocities[_currentVelocityIndex]);
    }
}
