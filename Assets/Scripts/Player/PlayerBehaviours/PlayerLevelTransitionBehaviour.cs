using UnityEngine;

public class PlayerLevelTransitionBehaviour : BasePlayerBehaviour
{

    private bool _startedTravel = false;
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
        _startedTravel = false;
        PlayerController.MovementController.enabled = false;
        PlayerController.MovementController.SetVelocity(Vector2.zero);
        PlayAnim(PlayerController.PlayerAnimator.AnimationList.LevelTransitionStart);

    }

    public override void Exit()
    {
        PlayerController.MovementController.enabled = true;
        PlayerController.PlaySFX(PlayerController.SoundList.travelEndSFX);
        PlayAnim(PlayerController.PlayerAnimator.AnimationList.LevelTransitionEnd);

    }

    public override void FixedUpdate(float delta)
    {

    }

    public override void Update(float delta)
    {
        if(PlayerController.GameManager.TravelingToLevel & !_startedTravel)
        {
            PlayerController.PlaySFX(PlayerController.SoundList.travelStartSFX);
            _startedTravel = true;
        }
    }
}
