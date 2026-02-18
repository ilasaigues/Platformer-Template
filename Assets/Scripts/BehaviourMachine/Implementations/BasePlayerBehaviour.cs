public abstract class BasePlayerBehaviour : BaseBehaviour
{
    protected PlayerController PlayerController { get; set; }
    public BasePlayerBehaviour(PlayerController player)
    {
        PlayerController = player;
    }
}