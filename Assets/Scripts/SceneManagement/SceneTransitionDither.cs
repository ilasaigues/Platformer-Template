using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SceneTransitionDither : SceneTransition
{
    private static readonly int DitherInHash = Animator.StringToHash("dither_in");
    private static readonly int DitherOutHash = Animator.StringToHash("dither_out");
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override async Task EnterAsync(Canvas target)
    {
        animator.Play(DitherInHash, 0);
        Debug.Log(animator.GetCurrentAnimatorStateInfo(0).length);
        await Task.Delay((int)(animator.GetCurrentAnimatorStateInfo(0).length * 1000));
    }

    public override async Task ExitAsync(Canvas target)
    {
        animator.Play(DitherOutHash, 0);
        await Task.Delay((int)(animator.GetCurrentAnimatorStateInfo(0).length * 1000));
    }

}
