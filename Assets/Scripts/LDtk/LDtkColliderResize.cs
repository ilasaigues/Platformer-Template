using UnityEngine;
using LDtkUnity;

public partial class LDtkColliderResize : MonoBehaviour, ILDtkImportedEntity
{
    public void OnLDtkImportEntity(EntityInstance entityInstance)
    {
        transform.localScale = Vector2.one;
        Vector2 LDtkSize = GetComponent<LDtkComponentEntity>().Size;
        Vector2 LDtkOffset = new Vector2(LDtkSize.x/2, -LDtkSize.y/2);
        if(TryGetComponent(out BoxCollider2D collider))
        {
            collider.size = LDtkSize;
            collider.offset = LDtkOffset;
        }
        if(TryGetComponent(out SpriteRenderer sprite))
        {
            sprite.size = LDtkSize;
        }
        if(TryGetComponent(out ParticleSystem particles))
        {
           var particleShape = particles.shape;
           particleShape.scale = LDtkSize;
           particleShape.position = LDtkOffset;
        }
    }
}
