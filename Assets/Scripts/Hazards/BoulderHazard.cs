using System;
using UnityEngine;
[RequireComponent(typeof(TimeContext))]
[RequireComponent(typeof(ObjectMovementComponent))]
public class BoulderHazard : MonoBehaviour
{
    public Vector2[] Velocities;
    private int _currentVelocityIndex = 0;
    ObjectMovementComponent _movementComponent;
    private TimeContext _timeContext;

    public float StopTime = 1;

    private float _stopTimer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _timeContext = GetComponent<TimeContext>();
        _movementComponent = GetComponent<ObjectMovementComponent>();
        _movementComponent.OnPlayerSqueezed += PlayerSqueezed;
        _movementComponent.OnObstacleHit += ObstacleHit;
    }

    private void ObstacleHit()
    {
        _currentVelocityIndex = (_currentVelocityIndex + 1) % Velocities.Length;
        _movementComponent.SetVelocity(Vector2.zero);
        _stopTimer = StopTime;
    }

    private void PlayerSqueezed(PlayerController controller)
    {
        controller.MarkAsDead();
        controller.GainAbility<PlayerRockBehaviour>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_stopTimer > 0)
        {
            _movementComponent.SetVelocity(Vector2.zero);
            _stopTimer -= _timeContext.FixedDeltaTime;
        }
        else
        {
            _movementComponent.SetVelocity(Velocities[_currentVelocityIndex]);
        }
    }
}
