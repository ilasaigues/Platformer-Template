using UnityEngine;

public class InputTester : MonoBehaviour
{
    public InputHandler inputHandler;
    CollisionTester _collisionTester;
    SpriteRenderer _sprite;

    void Awake()
    {
        _collisionTester = GetComponent<CollisionTester>();
        _sprite = GetComponent<SpriteRenderer>();
        inputHandler.MoveEvent += handleMove;
        inputHandler.JumpPressed += handleJumpPress;
        inputHandler.JumpReleased += handleJumpRealease;
    }


    void handleJumpPress()
    {
        _sprite.color = Color.red;
    }

    void handleJumpRealease()
    {
        _sprite.color = Color.green;
    }

    void handleMove(Vector2 direction)
    {
        _collisionTester.movementDirection = direction;
    }

}
