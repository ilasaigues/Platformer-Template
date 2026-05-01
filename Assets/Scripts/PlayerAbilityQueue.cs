using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityQueue
{
    private Queue<IPlayerAbilityBehaviour> _abilityQueue = new();

    public int MaxAbilityStack = 2;

    public event Action<IPlayerAbilityBehaviour> OnPlayerAbilityEnqueued = delegate { };
    public event Action<IPlayerAbilityBehaviour> OnPlayerAbilityDequeued = delegate { };

    public void AddAbility(IPlayerAbilityBehaviour ability)
    {
        if (_abilityQueue.Contains(ability)) return;
        //Debug.Log("Adding ability to queue: " + ability.GetType());
        _abilityQueue.Enqueue(ability);
        ability.Enabled = true;
        OnPlayerAbilityEnqueued(ability);
        while (_abilityQueue.Count > MaxAbilityStack)
        {
            var dequeue = _abilityQueue.Dequeue();
            dequeue.Enabled = false;
            OnPlayerAbilityDequeued(dequeue);
        }
    }
}
