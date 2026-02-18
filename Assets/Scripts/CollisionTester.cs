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
        var horizontalCollisons = _collisionController.GetHorizontalCollisions(movementDirection, Mathf.Abs(movementDirection.x), GroundLayerMask);
        foreach (var collisionOrigin in horizontalCollisons)
        {
            Debug.DrawRay(collisionOrigin, Vector2.right * movementDirection.x);
        }

        var verticalCollisions = _collisionController.GetVerticalCollisions(movementDirection, Mathf.Abs(movementDirection.y), GroundLayerMask);
        foreach (var collisionOrigin in verticalCollisions)
        {
            Debug.DrawRay(collisionOrigin, Vector2.up * movementDirection.y);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, movementDirection);
    }
}
