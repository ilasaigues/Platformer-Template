using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class DoorTrigger : MonoBehaviour
{

    public ScriptableEvent DoorTriggerEvent;

    void Start()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() is PlayerController)
        {
            TriggerDoor();
        }
    }

    void TriggerDoor()
    {
        DoorTriggerEvent.Raise();
    }
}
