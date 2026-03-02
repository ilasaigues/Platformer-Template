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

    public int MaxJumps => 1 + PlayerStats.extraJumps;

    public int RemainingJumps = 1;
    public int RemainingDashes = 1;
    public SpriteRenderer SpriteRenderer;

    public bool TryJump()
    {
        Debug.Log("Trying jump | Jumps left: " + RemainingJumps);
        return RemainingJumps > 0;
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
        RemainingJumps = MaxJumps;
    }

    private void ResetDashes()
    {
        RemainingDashes = 1;
    }

    public BehaviourChangeRequest TryUseAbility<T>(BasePlayerBehaviour currentBehaviour) where T : BasePlayerAbilityBehaviour
    {
        var ability = BehaviourMachine.GetBehaviour<T>();
        if (ability != null && ability.Enabled && !ability.OnCooldown && ability.CanTransitionFromBehaviour(currentBehaviour))
        {
            return BehaviourChangeRequest.New<T>();
        }
        return null;
    }

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
        BehaviourMachine.ChangeBehaviour(typeof(PlayerFallingBehaviour));
        ResetJumps();
    }
}