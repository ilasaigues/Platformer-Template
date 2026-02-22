using UnityEngine;

public class CollideAndSlide : MonoBehaviour
{
    public InputHandler InputHandler;
    public int maxBounces = 5;
    public float movementSpeed;
    public float SkinWidth = 0.01f;
    public LayerMask groundLayer;
    private Rigidbody2D _rb;
    private Vector2 currentDirection;
    private Collider2D _collider;
    private Vector2 currentVelocity;
    public float gravity;
    public float jumpvelocity;
    

    void Awake()
    {
        InputHandler.MoveEvent += GetInput;
        InputHandler.JumpPressed += Jump;
        InputHandler.JumpReleased += CutJump;
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        HandleMovement(Time.fixedDeltaTime);
    }
    void HandleMovement(float delta)
    {
        currentVelocity.x = currentDirection.normalized.x * movementSpeed;
        currentVelocity.y -= CurrentGravity() * delta;
        Vector2 frameVelocity;
        frameVelocity.x = CollideAndSlideVel(Vector2.right* currentVelocity.x * delta,transform.position,0).x;
        frameVelocity.y = CollideAndSlideVel(Vector2.up* currentVelocity.y * delta,transform.position,0).y;
        _rb.position += frameVelocity;
        currentVelocity = frameVelocity/delta;
        Debug.Log(currentVelocity.y);
    }

    void GetInput(Vector2 value)
    {
        currentDirection = value;
    }

    void Jump()
    {
        currentVelocity.y = jumpvelocity;
    }

    void CutJump()
    {
        
    }

    Vector2 CollideAndSlideVel(Vector2 vel, Vector2 pos, int depth)
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(-2*SkinWidth);
        if(depth >= maxBounces) return Vector2.zero;
        float dist = vel.magnitude + SkinWidth;

        RaycastHit2D hit = Physics2D.BoxCast(pos, bounds.size,0,vel.normalized, dist,groundLayer);

        if (hit)
        {
            Vector2 snapToSurface = vel.normalized * (hit.distance-SkinWidth);
            Vector2 leftover = vel - snapToSurface;
            if(snapToSurface.magnitude<=SkinWidth) snapToSurface = Vector2.zero;
            float mag = leftover.magnitude;
            leftover = Vector3.ProjectOnPlane(leftover, hit.normal).normalized;
            leftover*= mag;
            return snapToSurface + CollideAndSlideVel(leftover, pos + snapToSurface, depth+1);
        }
    
        return vel;
    }

    public float CurrentGravity()
    {
        if(currentVelocity.y > 2) return gravity;
        if(currentVelocity.y < 2) return gravity * 1.5f;
        return gravity * 0.5f;
        
    }
}
