using System;
using UnityEngine;

[RequireComponent(typeof(ScriptableEventListener))]
public class DoorBehaviour : MonoBehaviour
{

    public void TriggerDoor()
    {
        gameObject.SetActive(false);
    }
}
