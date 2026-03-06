using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    public PlayerAnimationList AnimationList;

    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }


    public void Play(AnimationClip animClip)
    {
        if (animClip == null) return;
        if (_animator.GetCurrentAnimatorClipInfo(0).First().clip != animClip)
        {
            _animator.Play(animClip.name);
        }
    }

}