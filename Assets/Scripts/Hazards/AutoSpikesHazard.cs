using UnityEngine;
using LDtkUnity;

[RequireComponent(typeof(TimeContext))]
[RequireComponent(typeof(Animator))]
public class AutoSpikesHazard : BaseHazard, ILDtkImportedFields
{

    public float TimeOffset;
    public float DownTime = 1;
    public float UpTime = 1;

    private bool _isUp;

    private float _innerTimer;
    private Animator _animator;
    private TimeContext _timeContext;

    private float _timer => _innerTimer - TimeOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _timeContext = GetComponent<TimeContext>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _innerTimer += _timeContext.FixedDeltaTime;
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

    public void OnLDtkImportFields(LDtkFields fields)
    {
        transform.localScale = Vector3.one;
        Vector2 newSize = GetComponent<LDtkComponentEntity>().Size;
        switch (fields.GetEnum<Orientation>("direction"))
        {
            case Orientation.Up:
                transform.rotation = Quaternion.Euler(Vector3.zero);
                break;
            case Orientation.Down:
                transform.rotation = Quaternion.Euler(new Vector3(180, 0, 0));
                transform.position += Vector3.up;
                break;
            case Orientation.Left:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                transform.position += Vector3.right;
                newSize.x = newSize.y;
                newSize.y = 1;
                break;
            case Orientation.Right:
                transform.rotation = Quaternion.Euler(new Vector3(0, 180, 90));
                //transform.position += Vector3.left;
                newSize.x = newSize.y;
                newSize.y = 1;
                break;
        }
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(newSize.x, 6.ToPixels());
        collider.offset = collider.size / 2;
        GetComponent<SpriteRenderer>().size = newSize;
        UpTime = fields.GetFloat("upTime");
        DownTime = fields.GetFloat("downTime");
        TimeOffset = fields.GetFloat("offset");
    }


}
