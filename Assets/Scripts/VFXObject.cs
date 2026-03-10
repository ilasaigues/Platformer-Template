
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class VFXObject : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    public UnityEvent<VFXObject> OnAnimationEnd;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sortingOrder = 1;
        _animator = GetComponent<Animator>();
    }

    public void SetDataAndPlay(AnimationClip animClip, Vector2 position, bool flipX)
    {
        transform.position = position;
        _spriteRenderer.flipX = flipX;
        _animator.Play(animClip.name);
    }

    public void EndAnimation()
    {
        OnAnimationEnd.Invoke(this);
    }

}