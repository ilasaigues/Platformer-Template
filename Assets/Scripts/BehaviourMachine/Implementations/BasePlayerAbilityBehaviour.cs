
using System;
using System.Linq;

public abstract class BasePlayerAbilityBehaviour : BasePlayerBehaviour
{

    public bool Enabled;
    public abstract bool OnCooldown { get; }
    public abstract Type[] TransitionableBehaviourTypes { get; }
    protected DateTime _timeLastUsed = default;

    public bool CanTransitionFromBehaviour(BasePlayerBehaviour behaviour)
    {
        return TransitionableBehaviourTypes.Any(t => t.IsAssignableFrom(behaviour.GetType()));
    }


    public override void Enter()
    {
        _timeLastUsed = DateTime.Now;
    }

    protected BasePlayerAbilityBehaviour(PlayerController player) : base(player)
    {
    }
}