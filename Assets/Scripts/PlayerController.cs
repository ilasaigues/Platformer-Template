using System;
using UnityEngine;
[RequireComponent(typeof(CollisionController))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(BehaviourMachine))]
public class PlayerController : MonoBehaviour
{
    public PlayerStats PlayerStats;
    public AbilityStats AbilityStats;
    public CollisionController CollisionController;
    public MovementController MovementController;

    public InputHandler InputHandler;

    public BehaviourMachine BehaviourMachine;

    public Vector2 LastDirectionInput => InputHandler.MoveAxis.LastValue;

    public int RemainingDashes = 1;

    public int Jumps = 0;

    public SpriteRenderer SpriteRenderer;

    void Start()
    {
        SpriteRenderer = gameObject.GetOrAddComponent<SpriteRenderer>();
        CollisionController = gameObject.GetOrAddComponent<CollisionController>();
        MovementController = gameObject.GetOrAddComponent<MovementController>();
        BehaviourMachine = gameObject.GetOrAddComponent<BehaviourMachine>();
        BehaviourMachine.AddBehaviour(new PlayerFallingBehaviour(this));
        BehaviourMachine.AddBehaviour(new PlayerIdleBehaviour(this));
        BehaviourMachine.AddBehaviour(new PlayerGroundMoveBehaviour(this));
        BehaviourMachine.AddBehaviour(new PlayerJumpingBehaviour(this));
        BehaviourMachine.AddBehaviour(new PlayerRockBehaviour(this)
        {
            Enabled = true
        });
        BehaviourMachine.AddBehaviour(new PlayerDashBehaviour(this)
        {
            Enabled = true
        });
        BehaviourMachine.AddBehaviour(new PlayerDoubleJumpBehaviour(this)
        {
            Enabled = true
        });
        BehaviourMachine.ChangeBehaviour(typeof(PlayerFallingBehaviour));
        ResetOnGrounded();
    }

    void OnEnable()
    {
        InputHandler.JumpButton.OnPress += OnJumpPressed;
    }


    void OnDisable()
    {
        InputHandler.JumpButton.OnPress -= OnJumpPressed;
    }

    private void OnJumpPressed()
    {
        if (MovementController.Grounded &&
            MovementController.OnOneWayPlatform &&
            InputHandler.MoveAxis.LastValue.y < 0 &&
            Mathf.Abs(InputHandler.MoveAxis.LastValue.y) >= Mathf.Abs(InputHandler.MoveAxis.LastValue.x))
        {
            InputHandler.JumpButton.Pressed = false;
            InputHandler.JumpButton.JustPressed = false;
            InputHandler.JumpButton.TimeLastPressed = DateTime.Now - TimeSpan.FromSeconds(PlayerStats.jumpBufferTime * 2);
            MovementController.IgnoreOneWay = true;
        }
    }

    public bool TryDash()
    {
        return RemainingDashes > 0;
    }

    public void ResetOnGrounded()
    {
        ResetJumps();
        ResetDashes();
    }

    private void ResetJumps()
    {
        Jumps = 0;
    }

    private void ResetDashes()
    {
        RemainingDashes = 1;
    }

    public BehaviourChangeRequest TryUseAbility<T>(BasePlayerBehaviour currentBehaviour) where T : BaseBehaviour, IPlayerAbilityBehaviour
    {
        var ability = BehaviourMachine.GetBehaviour<T>();
        if (ability != null && ability.Enabled && !ability.OnCooldown)
        {
            return BehaviourChangeRequest.New<T>();
        }
        return null;
    }


}