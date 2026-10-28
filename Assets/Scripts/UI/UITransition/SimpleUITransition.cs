using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Simple UI Transition", menuName = "Scriptable Objects/UI/Simple UI Transition")]
public class SimpleUITransition : BaseUITransition
{
    public Vector3 StartPosition;
    public Vector3 EndPosition;

    [Range(0, 1)] public float TargetAlpha = 1;
    public float TransitionTime;

    public LeanTweenType TweenType;
    public async override Task DoTransition(GameObject UIObject)
    {
        UIObject.transform.localPosition = StartPosition;
        var tween = LeanTween.moveLocal(UIObject, EndPosition, TransitionTime).setEase(TweenType);
        LeanTween.alphaCanvas(UIObject.GetComponent<CanvasGroup>(), TargetAlpha, TransitionTime).setEase(TweenType);
        bool complete = false;
        tween.setOnComplete(_ => complete = true);
        await UniTask.WaitUntil(() => complete);
    }
}
