using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityQueue
{
    private Queue<IPlayerAbilityBehaviour> _abilityQueue = new();

    public int MaxAbilityStack = 2;

    public void AddAbility(IPlayerAbilityBehaviour ability)
    {
        if (_abilityQueue.Contains(ability)) return;
        Debug.Log("Adding ability to queue: " + ability.GetType());
        _abilityQueue.Enqueue(ability);
        ability.Enabled = true;
        while (_abilityQueue.Count > MaxAbilityStack)
        {
            _abilityQueue.Dequeue().Enabled = false;
        }
    }
}
