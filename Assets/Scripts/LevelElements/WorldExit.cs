using System;
using UnityEngine;

public class WorldExit : MonoBehaviour
{
    public event Action<LevelTraversalComponent> OnWorldExitTriggered;

    bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggered && other.GetComponent<LevelTraversalComponent>() is LevelTraversalComponent traveller)
        {
            triggered = true;
            OnWorldExitTriggered(traveller);
        }
    }
}
