using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class TestBehaviourDisplay : MonoBehaviour
{
    [SerializeField]
    BehaviourMachine _behaviourMachine;
    UIDocument _ui;

    Label _label;
    void Start()
    {
        _ui = GetComponent<UIDocument>();
        _label = _ui.rootVisualElement.Q<Label>("CurrentStateLabel");

    }
    void Update()
    {
        _label.text = _behaviourMachine.GetBehaviourName;
    }
}
