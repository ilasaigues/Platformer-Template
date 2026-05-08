using UnityEngine;
using Zenject;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemFramerate : MonoBehaviour
{
    [Inject]
    TimeContext _timeContext;

    ParticleSystem _particleSystem;

    float _elapsedTime;


    [SerializeField]
    [Range(1, 60)]
    int _framerate;
    float _frameTime => 1f / _framerate;

    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    void FixedUpdate()
    {
        _elapsedTime += _timeContext.FixedDeltaTime;

        if (_elapsedTime >= _frameTime)
        {
            _particleSystem.Simulate(_frameTime, true, false, false);
            _elapsedTime -= _frameTime;
        }

    }
}
