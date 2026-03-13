using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/VFX Spawn Data")]
public class VFXSpawnData : ScriptableObject
{
    public AnimationClip VFXClip;
    public Vector3 Offset;
}
