using UnityEngine;

public static class MonobehaviourExtensions
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        //Attempt to find the component on the object
        //If it doesn't exist, create a new one
        T newComponent = go.GetComponent<T>() ?? go.AddComponent<T>();

        //Return the component
        return newComponent;
    }
}
