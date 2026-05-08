using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollisionController : MonoBehaviour
{

    public BoxCollider2D MainCollider;
    public BoxCollider2D FootCollider;

    private const float FootColliderHeight = 0.125f;

    void Start()
    {
        ResizeFloorCollider();
    }

    public void ResizeMainCollider(Vector2 size, Vector2 offset)
    {
        MainCollider.offset = offset;
        MainCollider.size = size;
        ResizeFloorCollider();
    }

    private void ResizeFloorCollider()
    {
        FootCollider.offset = Vector2.up * (MainCollider.offset.y + (-MainCollider.size.y + FootColliderHeight) * 0.5f);
        FootCollider.size = new Vector2(MainCollider.size.x, FootColliderHeight);
    }

}
