using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
[System.Serializable]

public abstract class BaseUITransition : ScriptableObject
{
    public abstract Task DoTransition(GameObject UIObject);
}
