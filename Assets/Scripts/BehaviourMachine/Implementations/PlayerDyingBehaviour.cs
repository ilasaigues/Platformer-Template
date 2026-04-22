using System.Threading;
using UnityEngine;

public class PlayerDyingBehaviour : BasePlayerBehaviour
{

    private float _timeRemaining;

    private bool _isDying;

    private bool _startedMovementTransition;

    public PlayerDyingBehaviour(PlayerController player) : base(player)
    {
    }

    public override void Enter()
    {
        _isDying = true;
        _startedMovementTransition = false;
        _timeRemaining = PlayerController.PlayerStats.TotalDeathTime;
        PlayerController.CollisionController.MainCollider.enabled = false;
        PlayAnim(PlayerController.PlayerAnimator.AnimationList.Death);
        PlayerController.MovementController.SetVelocity(Vector2.zero);
        Vector2 particlePosition = PlayerController.CurrentRespawnTrigger.RespawnPosition + Vector3.up * 8.ToPixels();
        VFXSpawner.Instance.PlayFX(VFXSpawner.Instance.VFXList.Respawn_Particles,particlePosition , 1,false);
    }

    public override void Exit()
    {
        PlayerController.CollisionController.MainCollider.enabled = true;
    }

    public override void FixedUpdate(float delta)
    {
        // no op
    }

    public override void Update(float delta)
    {
        // change to respawn position (and stick to ground) and animation after death duration


        if (!_startedMovementTransition)
        {
            _startedMovementTransition = true;
            LeanTween.move(
                PlayerController.gameObject,
                PlayerController.CurrentRespawnTrigger.RespawnPosition,
                PlayerController.PlayerStats.DeathDuration * .5f)
                .setEaseOutQuad().setDelay(PlayerController.PlayerStats.DeathDuration * .75f);
        }


        if (_isDying && _timeRemaining <= PlayerController.PlayerStats.ReviveDuration)
        {
            _isDying = false;
            PlayerController.Respawn();
            PlayAnim(PlayerController.PlayerAnimator.AnimationList.Revive);
        }

        _timeRemaining -= delta;
    }

    public override BehaviourChangeRequest VerifyBehaviour()
    {
        if (base.VerifyBehaviour() is BehaviourChangeRequest baseRequest)
        {
            return baseRequest;
        }

        if (_timeRemaining <= 0)
        {
            return BehaviourChangeRequest.New<PlayerJumpingBehaviour>();
        }

        return null;
    }
}
