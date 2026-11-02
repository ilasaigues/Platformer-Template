using UnityEngine;
using LDtkUnity;

public class ThornsHazard : BaseHazard, ILDtkImportedFields
{
    public void OnLDtkImportFields(LDtkFields fields)
    {
        Vector2 newSize = GetComponent<LDtkComponentEntity>().Size;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        colliderReduction = ((int)colliderReduction).ToPixels();
        transform.localScale = Vector3.one;
        collider.size = newSize - Vector2.one * colliderReduction;
        collider.offset = collider.size * new Vector2(0.5f, -0.5f) + new Vector2(0.5f,-0.5f) * colliderReduction;
        GetComponent<SpriteRenderer>().size = newSize;

    }
}
