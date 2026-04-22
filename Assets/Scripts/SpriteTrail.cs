using UnityEngine;

[RequireComponent(typeof(TimeContext))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class SpriteTrail : MonoBehaviour
{

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private Transform _targetTransform;
    private Vector2 _worldPosition;

    public void StartTrail(Vector2 position, Transform targetTransform)
    {
        _spriteRenderer ??= GetComponent<SpriteRenderer>();
        _animator ??= GetComponent<Animator>();
        gameObject.SetActive(true);
        _worldPosition = transform.position = position;
        _animator.Play("external_trail");
        _targetTransform = targetTransform;
    }

    public void StopTrail()
    {
        gameObject.SetActive(false);
        _targetTransform = null;
    }

    void Update()
    {
        if (_targetTransform == null) return;
        transform.position = _worldPosition;
        float horizontalDistance = (_targetTransform.position - transform.position).x;
        _spriteRenderer.flipX = false;
        transform.localScale = Vector3.one;
        if (horizontalDistance < 0)
        {
            horizontalDistance *= -1;
            _spriteRenderer.flipX = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        _spriteRenderer.size = new Vector2(horizontalDistance, 1);
    }

}
