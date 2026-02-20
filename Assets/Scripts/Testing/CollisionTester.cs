using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CollisionController))]
public class CollisionTester : MonoBehaviour
{

    public Vector2 movementDirection;

    public LayerMask GroundLayerMask;
    CollisionController _collisionController;
    void Awake()
    {
        _collisionController = GetComponent<CollisionController>();
    }

    void Update()
    {
        var boxBounds = GetComponent<Collider2D>().bounds;



        var horizontalHits = Physics2D.BoxCastAll(boxBounds.center, boxBounds.size, 0, Vector2.right, movementDirection.x, GroundLayerMask);
        //.Where(hit => hit.fraction != 0);

        Debug.DrawLine(boxBounds.min, boxBounds.max);

        foreach (var hit in horizontalHits)
        {
            Debug.DrawLine(hit.point, boxBounds.center, Color.blue);
        }

        var verticalHits = Physics2D.BoxCastAll(boxBounds.center, boxBounds.size, 0, Vector2.up, movementDirection.y, GroundLayerMask);
        //.Where(hit => hit.fraction != 0);
        foreach (var hit in verticalHits)
        {
            //Debug.DrawLine(hit.point, boxBounds.center, Color.green);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var boxBounds = GetComponent<Collider2D>().bounds;
        Gizmos.DrawWireCube((Vector2)boxBounds.center + (Vector2.right * movementDirection.x), boxBounds.size);
        Gizmos.DrawWireCube((Vector2)boxBounds.center + (Vector2.up * movementDirection.y), boxBounds.size);
        Gizmos.DrawRay(transform.position, movementDirection);
    }
}
