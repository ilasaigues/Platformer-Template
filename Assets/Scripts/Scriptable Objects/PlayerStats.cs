using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Grounded")]
    public float groundedSpeed;
    public float groundedAccelerationTime;
    public float groundedDecelerationTime;
    [Header("Air")]
    public float airSpeed;
    public float airAccelerationTime;
    public float airDecelerationTime;
    [Header("Jump")]
    public float jumpVelocity;
    public float jumpGravity;
    public float peakGravity;
    public float fallGravity;
    public float cutoffGravity;
    public float fallVelocityCap;
    public Vector2 peakTresholds;
    public float coyoteTime;
    public float jumpBufferTime;
    [Header("Bonuses")]
    public float speedBonus;
    public float jumpBonus;
    public int extraJumps;
    public float gravityMult;
    [Header("Adjustments")]
    public float ceilingCorrection;
    public float ledgeCorrection;

}
