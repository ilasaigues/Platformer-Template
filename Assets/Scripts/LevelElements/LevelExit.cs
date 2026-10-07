
using System;
using UnityEngine;
using Zenject;

public class LevelExit : MonoBehaviour
{
    public event Action<LevelTraversalComponent> OnExitTriggered;

    bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggered && other.GetComponent<LevelTraversalComponent>() is LevelTraversalComponent traveller)
        {
            triggered = true;
            OnExitTriggered(traveller);
        }
    }
}
