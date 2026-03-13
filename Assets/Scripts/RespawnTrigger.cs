using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class RespawnTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform _respawnTransform;

    public Vector3 RespawnPosition => _respawnTransform ? _respawnTransform.position : Vector3.zero;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() is PlayerController controller)
        {
            controller.SetRespawn(this);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (_respawnTransform != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, _respawnTransform.position);
        }
    }

}
