using UnityEngine;
using LDtkUnity;

public class LDtkColliderResize : MonoBehaviour, ILDtkImportedEntity
{
    public void OnLDtkImportEntity(EntityInstance entityInstance)
    {
        transform.localScale = Vector2.one;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = GetComponent<LDtkComponentEntity>().Size;
        collider.offset = new Vector2(collider.size.x, -collider.size.y)/2;      
    }
}
