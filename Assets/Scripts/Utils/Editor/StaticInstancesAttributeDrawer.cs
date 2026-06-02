using System;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(StaticInstancesAttribute))]
public class StaticInstancesAttributeDrawer : PropertyDrawer
{

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // get type
        var propertyType = property.GetUnderlyingType();
        // get all static instances of same type
        FieldInfo[] fields = propertyType.GetFields(BindingFlags.Static | BindingFlags.Public);

        // get current prop value
        var value = property.boxedValue;


        var valueArray = fields.Select(f => f.GetValue(null)).ToList();
        var index = -1;
        index = valueArray.IndexOf(value);

        index = EditorGUI.Popup(position, "Scene", index, fields.Select(f => f.Name).ToArray());

        if (index != -1)
        {
            property.boxedValue = fields[index].GetValue(null);
        }

        property.serializedObject.ApplyModifiedProperties();
    }
}
