
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class VFXObject : MonoBehaviour
{
    [Inject]
    TimeContext _timeContext;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    public UnityEvent<VFXObject> OnAnimationEnd;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sortingOrder = 1;
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _timeContext.CreateContextModules(gameObject);
    }

    public void SetDataAndPlay(AnimationClip animClip, Vector2 position, int order, bool flipX)
    {
        transform.position = position;
        _spriteRenderer.sortingOrder = order;
        _spriteRenderer.flipX = flipX;
        _animator.Play(animClip.name);
    }

    public void EndAnimation()
    {
        OnAnimationEnd.Invoke(this);
    }

    public class Factory : PlaceholderFactory<VFXObject>
    {
    }

}