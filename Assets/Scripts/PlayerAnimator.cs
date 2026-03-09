using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    public PlayerAnimationList AnimationList;

    private AnimationClip _currentAnimation;

    private Animator _animator;


    private Queue<AnimationClip> _animationQueue = new();


    void Awake()
    {
        _animator = GetComponent<Animator>();
    }


    public void PlayAnimationClip(AnimationClip animClip, bool clearQueue = true)
    {
        if (animClip == null) return;
        Debug.Log("<color=blue>Play " + animClip.name);
        if (clearQueue) _animationQueue.Clear();
        _currentAnimation = animClip;
        _animator.Play(animClip.name);

    }

    public void EnqueueAnimationClip(AnimationClip clip)
    {

        if (_animationQueue.Count == 0 && CanOverrideCurrentAnim())
        {
            PlayAnimationClip(clip, false);
        }
        else
        {
            Debug.Log("<color=cyan>Enqueue " + clip.name);
            _animationQueue.Enqueue(clip);
        }
    }

    private bool CanOverrideCurrentAnim()
    {
        var currentAnimatorState = _animator.GetCurrentAnimatorStateInfo(0);
        
        return _currentAnimation.isLooping || currentAnimatorState.normalizedTime >= 1;
    }

    void Update()
    {
        //Debug.Log("<color=red> Anim Queue: " + _animationQueue.Count);
        if (_animationQueue.Count > 0 && CanOverrideCurrentAnim())
        {
            PlayAnimationClip(_animationQueue.Dequeue(), false);
        }
    }

}