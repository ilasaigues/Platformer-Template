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

    public Vector2 LastDirectionInput;


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
    }

    void OnEnable()
    {
        InputHandler.MoveEvent += MoveEvent;
    }
    public void OnDisable()
    {
        InputHandler.MoveEvent -= MoveEvent;
    }
    private void MoveEvent(Vector2 vector)
    {
        LastDirectionInput = vector;
    }
}