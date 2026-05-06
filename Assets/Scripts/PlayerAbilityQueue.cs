using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityQueue
{
    public Queue<IPlayerAbilityBehaviour> AbilityQueue = new();

    public int MaxAbilityStack = 1;

    public event Action<IPlayerAbilityBehaviour> OnPlayerAbilityEnqueued = delegate { };
    public event Action<IPlayerAbilityBehaviour> OnPlayerAbilityDequeued = delegate { };

    public void AddAbility(IPlayerAbilityBehaviour ability)
    {
        if (AbilityQueue.Contains(ability)) return;
        //Debug.Log("Adding ability to queue: " + ability.GetType());
        AbilityQueue.Enqueue(ability);
        ability.Enabled = true;
        OnPlayerAbilityEnqueued(ability);
        while (AbilityQueue.Count > MaxAbilityStack)
        {
            var dequeue = AbilityQueue.Dequeue();
            dequeue.Enabled = false;
            OnPlayerAbilityDequeued(dequeue);
        }
    }
}
