using UnityEngine;
[RequireComponent(typeof(CollisionController))]
[RequireComponent(typeof(ColliderController))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(BehaviourMachine))]
public class PlayerController : MonoBehaviour
{
    public PlayerStats PlayerStats;
    public CollisionController CollisionController;
    public ColliderController ColliderController;
    public MovementController MovementController;

    public BehaviourMachine BehaviourMachine;

    void Start()
    {
        CollisionController = gameObject.GetOrAddComponent<CollisionController>();
        ColliderController = gameObject.GetOrAddComponent<ColliderController>();
        MovementController = gameObject.GetOrAddComponent<MovementController>();
        BehaviourMachine = gameObject.GetOrAddComponent<BehaviourMachine>();
        BehaviourMachine.AddBehaviour(new PlayerIdleBehaviour(this));
        BehaviourMachine.AddBehaviour(new PlayerFallingBehaviour(this));
        BehaviourMachine.ChangeBehaviour(typeof(PlayerFallingBehaviour));
    }
}