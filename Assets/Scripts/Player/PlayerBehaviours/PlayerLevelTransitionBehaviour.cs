using UnityEngine;

public class PlayerLevelTransitionBehaviour : BasePlayerBehaviour
{

    public override BehaviourChangeRequest VerifyBehaviour()
    {
        if (!PlayerController.GameManager.ChangingLevel)
        {
            if (PlayerController.MovementController.Grounded)
            {
                return BehaviourChangeRequest.New<PlayerIdleBehaviour>();
            }
            else
            {
                return BehaviourChangeRequest.New<PlayerFallingBehaviour>();
            }
        }
        return null;
    }

    public PlayerLevelTransitionBehaviour(PlayerController player) : base(player)
    {

    }

    public override void Enter()
    {
        PlayerController.MovementController.enabled = false;
        PlayerController.MovementController.SetVelocity(Vector2.zero);
        PlayAnim(PlayerController.PlayerAnimator.AnimationList.LevelTransitionStart);

    }

    public override void Exit()
    {
        PlayerController.MovementController.enabled = true;
        PlayAnim(PlayerController.PlayerAnimator.AnimationList.LevelTransitionEnd);

    }

    public override void FixedUpdate(float delta)
    {

    }

    public override void Update(float delta)
    {

    }
}
