using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Scriptable Event")]
public class ScriptableEvent : ScriptableObject
{
    public List<ScriptableEventListener> SubscribedListeners = new();

    public void Subscribe(ScriptableEventListener listener)
    {
        if (!SubscribedListeners.Contains(listener))
            SubscribedListeners.Add(listener);
    }

    public void Unsubscribe(ScriptableEventListener listener)
    {
        SubscribedListeners.Remove(listener);
    }

    public void Raise()
    {
        for (int i = SubscribedListeners.Count - 1; i >= 0; i--)
        {
            SubscribedListeners[i].OnEventRaised();
        }
    }

}

