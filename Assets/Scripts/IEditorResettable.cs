using UnityEngine;

public interface IEditorResettable
{
#if UNITY_EDITOR
    public void OnExitPlaymode();
    public void OnEnterPlaymode();
#endif
}