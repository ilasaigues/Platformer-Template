
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/VFX List", fileName = "new VFX List")]
public class VFXList : ScriptableObject
{
    public AnimationClip DashFeathers;
    public AnimationClip ShieldEnd;
    public AnimationClip AirBurst;
    public AnimationClip DashBurst;
    public AnimationClip Respawn_Particles;
    public AnimationClip AirRing;
    public AnimationClip JumpParticles_H;
    public AnimationClip JumpParticles_V;
    public AnimationClip LandParticles;
    public AnimationClip RunParticles;
}