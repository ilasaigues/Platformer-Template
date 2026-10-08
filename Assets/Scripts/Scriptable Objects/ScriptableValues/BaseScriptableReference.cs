using UnityEngine;

[System.Serializable]
public abstract class BaseScriptableReference<T>
{
    public enum ValueReferenceType
    {
        Local,
        Reference,
    }

    public T Value
    {
        get => ReferenceValue == null || ReferenceType == ValueReferenceType.Local ? LocalValue : ReferenceValue.Value;

        set
        {
            if (ReferenceValue == null || ReferenceType == ValueReferenceType.Local)
            { LocalValue = value; }
            else
            { ReferenceValue.Value = value; }
        }
    }
    public ValueReferenceType ReferenceType;
    public T LocalValue;
    public BaseScriptableValue<T> ReferenceValue;

    public static implicit operator T(BaseScriptableReference<T> v) => v.Value;
}
