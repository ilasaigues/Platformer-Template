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

    [SerializeField]
    private T _value;

    public event Action<T> OnValueChanged = delegate { };
    public event Action<T, T> OnValueChangedWithHistory = delegate { };


#if UNITY_EDITOR
    public string EditorDescription;

    public bool ChangedInPlaymode => !_editorSavedValue.Equals(_value);

    private T _editorSavedValue;
    public virtual void OnEnterPlaymode()
    {
        _editorSavedValue = _value;
    }

    public virtual void OnExitPlaymode()
    {
        OnValueChanged = delegate { };
        OnValueChangedWithHistory = delegate { };
        _value = _editorSavedValue;
    }

    public void ApplyPlayModeValueChange()
    {
        _editorSavedValue = _value;
    }

#endif
}
