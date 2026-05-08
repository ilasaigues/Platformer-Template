using UnityEngine;
using UnityEngine.Events;

public class ScriptableEventListener : MonoBehaviour
{
    public ScriptableEvent Event;
    public UnityEvent Response;

    private void OnEnable()
    {
        Event.Subscribe(this);
    }

    private void OnDisable()
    {
        Event.Unsubscribe(this);
    }

    public void OnEventRaised()
    {
        Response.Invoke();
    }
}