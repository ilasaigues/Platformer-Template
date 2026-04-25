using System;
using UnityEngine;

[Serializable]
public abstract class BaseScriptableValue<T> : ScriptableObject, IEditorResettable
{
    public T Value
    {
        get => _value;
        set
        {
            var oldValue = _value;
            _value = value;
            OnValueChanged(value);
            OnValueChangedWithHistory(_value, oldValue);

        }
    }

#if UNITY_EDITOR
    [ContextMenuItem("Apply Change", "ApplyValueChange")]
    [ContextMenuItem("Force Refresh", "ForceRefresh")]
#endif
    [SerializeField]
    private T _value;

    public event Action<T> OnValueChanged = delegate { };
    public event Action<T, T> OnValueChangedWithHistory = delegate { };


#if UNITY_EDITOR
    // Editor Specific utilities
    [TextArea(minLines: 3, maxLines: 5)]
    public string EditorDescription;

    public bool ChangedInPlaymode => !_editorSavedValue.Equals(_value);

    private T _editorSavedValue;
    public void OnEnterPlaymode()
    {
        _editorSavedValue = _value;
    }

    public void OnExitPlaymode()
    {
        OnValueChanged = delegate { };
        OnValueChangedWithHistory = delegate { };
        _value = _editorSavedValue;
    }

    public void ApplyValueChange()
    {
        _editorSavedValue = _value;
    }

    public void ForceRefresh()
    {
        Value = _value;
    }

#endif
}
