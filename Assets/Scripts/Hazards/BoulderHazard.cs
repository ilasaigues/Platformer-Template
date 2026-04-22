using System;
using System.Collections.Generic;
using System.Drawing;
using LDtkUnity;
using UnityEngine;
[RequireComponent(typeof(TimeContext))]
[RequireComponent(typeof(ObjectMovementComponent))]
public class BoulderHazard : MonoBehaviour, ILDtkImportedFields
{
    public Vector2[] Velocities;
    private int _currentVelocityIndex = 0;
    ObjectMovementComponent _movementComponent;
    private TimeContext _timeContext;

    public float StopTime = 1;
    private float _stopTimer = 0;

    private Vector2 _size;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _timeContext = GetComponent<TimeContext>();
        _movementComponent = GetComponent<ObjectMovementComponent>();
        _movementComponent.OnPlayerSqueezed += PlayerSqueezed;
        _movementComponent.OnObstacleHit += ObstacleHit;
    }

    void OnValidate()
    {

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

    public void OnLDtkImportFields(LDtkFields fields)
    {
        Dictionary<Orientation, Vector2> convertedValue = new()
        {
            { Orientation.Up, Vector2.up },
            { Orientation.Down, -Vector2.up },
            { Orientation.Left, -Vector2.right },
            { Orientation.Right, Vector2.right }
        };
        Velocities = new Vector2[fields.GetArraySize("boulderDirections")];
        for (int i = 0; i < Velocities.Length; i++)
        {
            Velocities[i] = convertedValue[fields.GetEnumArray<Orientation>("boulderDirections")[i]] * fields.GetFloat("speed");
        }
        StopTime = fields.GetFloat("stopTime");
        transform.localScale = Vector3.one;
        GetComponent<SpriteRenderer>().size = GetComponent<LDtkComponentEntity>().Size;
    }
}
