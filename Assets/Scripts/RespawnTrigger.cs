using LDtkUnity;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class RespawnTrigger : MonoBehaviour, ILDtkImportedFields
{
    [SerializeField]
    private Transform _respawnTransform;

    public RespawnType respawnType;

    public Vector3 RespawnPosition => _respawnTransform ? _respawnTransform.position : Vector3.zero;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() is PlayerController controller)
        {
            controller.SetRespawn(this, respawnType);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (_respawnTransform != null)
        {
            Gizmos.color = respawnType == RespawnType.Soft ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, _respawnTransform.position);
        }
    }

    public void OnLDtkImportFields(LDtkFields fields)
    {
        transform.localScale = Vector3.one;
        respawnType = fields.GetEnum<RespawnType>("type");
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = GetComponent<LDtkComponentEntity>().Size;
        collider.offset = new Vector2(collider.size.x, -collider.size.y) / 2;
        _respawnTransform = transform.GetChild(0);
    }
}
