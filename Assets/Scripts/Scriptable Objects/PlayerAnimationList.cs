
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/PlayerAnimationList", fileName = "new PlayerAnimationList")]
public class PlayerAnimationList : ScriptableObject
{
    public AnimationClip Idle;
    public AnimationClip Run;
    public AnimationClip Jump;
    public AnimationClip Peak;
    public AnimationClip Falling;
    public AnimationClip DoubleJump;
    public AnimationClip Land;
    public AnimationClip ShieldEnter;
    public AnimationClip Shield;
    public AnimationClip ShieldExit;
    public AnimationClip DashEnter;
    public AnimationClip Dash;
    public AnimationClip DashExit;

}