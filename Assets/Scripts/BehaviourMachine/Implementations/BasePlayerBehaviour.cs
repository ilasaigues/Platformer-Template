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
        if (PlayerController.IsDead)
        {
            return BehaviourChangeRequest.New<PlayerDyingBehaviour>();
        }

        if (PlayerController.InputHandler.RockButton.JustPressed)
        {
            return PlayerController.TryUseAbility<PlayerRockBehaviour>();
        }
        if (PlayerController.InputHandler.DashButton.JustPressed && PlayerController.TryDash())
        {
            return PlayerController.TryUseAbility<PlayerDashBehaviour>();
        }
        return null;
    }

    public void PlayAnim(AnimationClip animClip, bool clearQueue = true)
    {
        PlayerController.PlayerAnimator.PlayAnimationClip(animClip, clearQueue);
    }

    public void EnqueueAnim(AnimationClip animClip)
    {
        PlayerController.PlayerAnimator.EnqueueAnimationClip(animClip);
    }
}