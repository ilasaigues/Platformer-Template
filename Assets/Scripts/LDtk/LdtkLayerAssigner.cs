using LDtkUnity;
using UnityEngine;

public class LdtkLayerAssigner : LDtkUnity.Editor.LDtkPostprocessor
{
    protected override void OnPostprocessLevel(GameObject root, LdtkJson projectJson)
    {
        foreach (Transform layer in root.transform)
        {
            if (layer.name.Contains("Thorns"))
            {
                GameObject child = layer.transform.GetChild(0).gameObject;
                child.layer = (int)Mathf.Log(LayerReference.HazardLayer, 2);
                child.AddComponent<BaseHazard>().Type = BaseHazard.HazardType.DoubleJump;
            }
            
            if (layer.name.Contains("Doom"))
            {
                GameObject child = layer.transform.GetChild(0).gameObject;
                child.layer = (int)Mathf.Log(LayerReference.HazardLayer, 2);
                child.AddComponent<BaseHazard>().Type = BaseHazard.HazardType.Doom;
            }
        }
    }
}
