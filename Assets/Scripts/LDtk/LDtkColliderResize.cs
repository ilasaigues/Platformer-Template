using UnityEngine;
using LDtkUnity;

public class LDtkColliderResize : MonoBehaviour, ILDtkImportedEntity
{
    public void OnLDtkImportEntity(EntityInstance entityInstance)
    {
        transform.localScale = Vector2.one;
        Vector2 LDtkSize = GetComponent<LDtkComponentEntity>().Size;
        Vector2 LDtkOffset = new Vector2(LDtkSize.x/2, -LDtkSize.y/2);
        if(GetComponent<BoxCollider2D>() is BoxCollider2D collider)
        {
            collider.size = LDtkSize;
            collider.offset = LDtkOffset;
        }
        if(GetComponent<SpriteRenderer>() is SpriteRenderer sprite)
        {
            sprite.size = LDtkSize;
        }
        if(GetComponent<ParticleSystem>() is ParticleSystem particles)
        {
           var particleShape = particles.shape;
           particleShape.scale = LDtkSize;
           particleShape.position = LDtkOffset;
        }
    }
}
