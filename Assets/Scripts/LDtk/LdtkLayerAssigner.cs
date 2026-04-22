using LDtkUnity;
using LDtkUnity.Editor;
using UnityEngine;

public class LdtkLayerAssigner : LDtkPostprocessor
{
    protected override void OnPostprocessLevel(GameObject root, LdtkJson projectJson)
    {
        foreach(Transform layer in root.transform)
        {
            if (layer.name.Contains("Thorns"))
            {
                GameObject child = layer.transform.GetChild(0).gameObject;
                child.layer = 10;
                child.AddComponent<BaseHazard>().Type = BaseHazard.HazardType.DoubleJump;
            }
        }
    }
}
