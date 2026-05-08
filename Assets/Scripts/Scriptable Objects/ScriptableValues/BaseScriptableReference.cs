using UnityEngine;

[System.Serializable]
public abstract class BaseScriptableReference<T>
{
    public enum ValueReferenceType
    {
        Local,
        Reference,
    }

    public T Value => ReferenceValue == null || ReferenceType == ValueReferenceType.Local ? LocalValue : ReferenceValue.Value;
    public ValueReferenceType ReferenceType;
    public T LocalValue;
    public BaseScriptableValue<T> ReferenceValue;

    public static implicit operator T(BaseScriptableReference<T> v) => v.Value;
}
