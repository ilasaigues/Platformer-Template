using UnityEditor;
using UnityEditor.UI;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public abstract class BaseScriptableValueInspector<T> : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        // Create a new VisualElement to be the root of our Inspector UI.
        VisualElement myInspector = new();

        var overrideButton = new Button()
        {
            text = "Apply Change",
            enabledSelf = false,
        };

        overrideButton.clicked += () =>
        {
            ((BaseScriptableValue<T>)serializedObject.targetObject).ApplyValueChange();
            overrideButton.SetEnabled(false);
        };

        var serializedObjectField = GetObjectField(serializedObject);
        serializedObjectField.RegisterValueChangeCallback(changeEvent =>
        {
            var valueObject = serializedObject.targetObject as BaseScriptableValue<T>;
            valueObject.Value = valueObject.Value;
            if (EditorApplication.isPlaying)
            {
                overrideButton.SetEnabled(valueObject.ChangedInPlaymode);
            }
        });

        var descriptionLabel = new Label("Editor Description");
        var descriptionField = new TextField()
        {
            bindingPath = "EditorDescription",
            multiline = true,
        };
        descriptionField.style.whiteSpace = WhiteSpace.PreWrap;

        myInspector.Add(serializedObjectField);
        myInspector.Add(overrideButton);
        myInspector.Add(descriptionLabel);
        myInspector.Add(descriptionField);

        // Return the finished Inspector UI.
        return myInspector;
    }

    protected PropertyField GetObjectField(SerializedObject serializedObject)
    {
        var valueProperty = serializedObject.FindProperty("_value");

        var field = new PropertyField(valueProperty)
        {
            bindingPath = "_value",
        };
        return field;
    }
}
