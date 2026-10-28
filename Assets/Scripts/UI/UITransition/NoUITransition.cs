using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Empty UI Transition", menuName = "Scriptable Objects/UI/Empty UI Transition")]
public class NoUITransition : BaseUITransition
{
    public async override Task DoTransition(GameObject UIObject)
    {
        // no op
    }
}

