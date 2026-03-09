using UnityEngine;

public abstract class BasePlayerBehaviour : BaseBehaviour
{
    protected PlayerController PlayerController { get; set; }
    public BasePlayerBehaviour(PlayerController player)
    {
        PlayerController = player;
    }

    public override BehaviourChangeRequest VerifyBehaviour()
    {
        if (PlayerController.InputHandler.RockButton.JustPressed)
        {
            return PlayerController.TryUseAbility<PlayerRockBehaviour>(this);
        }
        if (PlayerController.InputHandler.DashButton.JustPressed && PlayerController.TryDash())
        {
            return PlayerController.TryUseAbility<PlayerDashBehaviour>(this);
        }
        return null;
    }

    public void PlayAnim(AnimationClip animClip, bool clearQueue = false)
    {
        PlayerController.PlayerAnimator.PlayAnimationClip(animClip, clearQueue);
    }

    public void EnqueueAnim(AnimationClip animClip)
    {
        PlayerController.PlayerAnimator.EnqueueAnimationClip(animClip);
    }
}