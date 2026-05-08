using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class TestAbilityGiver<T> : MonoBehaviour where T : BasePlayerBehaviour, IPlayerAbilityBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.isTrigger) return;
        if (collider.GetComponent<PlayerController>() is PlayerController controller)
        {
            controller.GainAbility<T>();
        }
    }
}
