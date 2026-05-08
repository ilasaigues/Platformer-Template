using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    public PlayerAnimationList AnimationList;


    private Animator _animator;

    private bool _startedPlayingThisframe;

    private Queue<AnimationClip> _animationQueue = new();


    void Awake()
    {
        _animator = GetComponent<Animator>();
    }


    public bool PlayAnimationClip(AnimationClip animClip, bool clearQueue = true)
    {
        if (animClip == null || (!clearQueue && _startedPlayingThisframe)) return false;
        //Debug.Log("<color=blue>Play " + animClip.name);
        if (clearQueue) _animationQueue.Clear();
        _animator.Play(animClip.name);
        _startedPlayingThisframe = true;
        return true;
    }

    public void EnqueueAnimationClip(AnimationClip clip)
    {
        //Debug.Log("<color=white>Try Enqueue " + clip.name);

        if (!_startedPlayingThisframe && _animationQueue.Count == 0 && CanOverrideCurrentAnim())
        {
            PlayAnimationClip(clip, false);
        }
        else
        {
            //Debug.Log("<color=cyan>Enqueue " + clip.name);
            _animationQueue.Enqueue(clip);
        }
    }

    private bool CanOverrideCurrentAnim()
    {
        var currentAnimatorState = _animator.GetCurrentAnimatorStateInfo(0);
        var currentAnimation = _animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        return currentAnimation.isLooping || currentAnimatorState.normalizedTime >= 1;
    }

    void Update()
    {
        if (_animationQueue.Count > 0 && CanOverrideCurrentAnim())
        {
            if (PlayAnimationClip(_animationQueue.Peek(), false))
            {
                _animationQueue.Dequeue();
            }
        }

        _startedPlayingThisframe = false;
    }

}