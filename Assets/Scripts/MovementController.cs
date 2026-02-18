using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : MonoBehaviour
{
    public Vector2 Velocity { get; private set; }
    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = gameObject.GetOrAddComponent<Rigidbody2D>();
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePosition;
    }

    void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + Velocity * Time.fixedDeltaTime);
    }

    public void AddVelocity(Vector2 added)
    {
        Velocity += added;
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        Velocity = newVelocity;
    }

    public void ForceOffset(Vector2 offset)
    {
        ForcePosition(_rb.position + offset);
    }

    public void ForcePosition(Vector2 newPosition)
    {
        _rb.MovePosition(newPosition);
    }

}
