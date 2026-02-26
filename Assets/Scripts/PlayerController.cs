using UnityEngine;
[RequireComponent(typeof(CollisionController))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(BehaviourMachine))]
public class PlayerController : MonoBehaviour
{
    public PlayerStats PlayerStats;
    public CollisionController CollisionController;
    public MovementController MovementController;

    public InputHandler InputHandler;

    public BehaviourMachine BehaviourMachine;

    public Vector2 LastDirectionInput => InputHandler.MoveAxis.LastValue;

    public int MaxJumps => 1 + PlayerStats.extraJumps;

    public int RemainingJumps = 1;

    public bool TryJump()
    {
        Debug.Log("Trying jump | Jumps left: " + RemainingJumps);
        if (RemainingJumps > 0)
        {
            RemainingJumps--;
            return true;
        }
        return false;
    }

    public void ResetJumps()
    {
        RemainingJumps = MaxJumps;
    }

    void Start()
    {
        CollisionController = gameObject.GetOrAddComponent<CollisionController>();
        MovementController = gameObject.GetOrAddComponent<MovementController>();
        BehaviourMachine = gameObject.GetOrAddComponent<BehaviourMachine>();
        BehaviourMachine.AddBehaviour(new PlayerFallingBehaviour(this));
        BehaviourMachine.AddBehaviour(new PlayerIdleBehaviour(this));
        BehaviourMachine.AddBehaviour(new PlayerGroundMoveBehaviour(this));
        BehaviourMachine.AddBehaviour(new PlayerJumpingBehaviour(this));
        BehaviourMachine.ChangeBehaviour(typeof(PlayerFallingBehaviour));
        ResetJumps();
    }
}