using UnityEngine;
using LDtkUnity;

public class LDtkColliderResize : MonoBehaviour, ILDtkImportedEntity
{
    public void OnLDtkImportEntity(EntityInstance entityInstance)
    {
        Vector2 newScale = GetComponent<LDtkComponentEntity>().Size;
        transform.localScale = newScale;
        transform.position += (Vector3)new Vector2(newScale.x, -newScale.y) / 2;
    }
}
