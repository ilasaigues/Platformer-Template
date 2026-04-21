using UnityEngine;

public class GameObjectListSubscriber : MonoBehaviour
{
    public GameObjectScriptableList ListObject;

    private void OnEnable()
    {
        ListObject?.AddElement(gameObject);
    }

    private void OnDisable()
    {
        ListObject?.RemoveElement(gameObject);
    }
}
