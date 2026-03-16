using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AutoSpikesHazard : BaseHazard
{

    public float TimeOffset;
    public float DownTime = 1;
    public float UpTime = 1;

    private bool _isUp;

    private float _innerTimer;
    private Animator _animator;

    private float _timer => _innerTimer - TimeOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _innerTimer += Time.fixedDeltaTime;
        if (!_isUp && _timer >= DownTime)
        {
            _isUp = true;
            _animator.Play("dash_spikes_on");
            Debug.DrawRay(transform.position, Vector2.up, Color.red, 0.2f);
        }

        if (_isUp && _timer >= DownTime + UpTime)
        {
            _isUp = false;
            _animator.Play("dash_spikes_off");
            Debug.DrawRay(transform.position, Vector2.up, Color.green, 0.2f);
            _innerTimer -= DownTime + UpTime;
        }
    }


}
