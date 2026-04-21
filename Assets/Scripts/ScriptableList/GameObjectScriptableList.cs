using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/GameObject List")]
public class GameObjectScriptableList : ScriptableObject, IEditorResettable
{
    [SerializeField]
    private List<GameObject> Elements;

    public ReadOnlyCollection<GameObject> ReadOnlyList
    {
        get
        {
            return Elements.AsReadOnly();
        }
    }

    public void AddElement(GameObject element)
    {
        if (!Elements.Contains(element))
        {
            Elements.Add(element);
        }
    }
    public void RemoveElement(GameObject element)
    {
        Elements.Remove(element);
    }

#if UNITY_EDITOR
    public void OnEnterPlaymode() { }

    public void OnExitPlaymode()
    {
        Elements.Clear();
    }
#endif
}
