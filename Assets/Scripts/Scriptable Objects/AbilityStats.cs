using UnityEngine;

[CreateAssetMenu(fileName = "AbilityStats", menuName = "Scriptable Objects/AbilityStats")]
public class AbilityStats : ScriptableObject
{
    [Header("Rock")]
    public float RockGravity;
    public float MinRockTime;
    public float RockBreakSpeed;
    [Header("Dash")]
    public float DashWindupTime;
    public float DashMovementTime;
    public float DashWinddownTime;

    public float DashEndMultiplier;

    public bool DashPause;

    public float DashDistance;
    public float DashCooldown;
    public float TotalDashTime => DashWinddownTime + DashMovementTime + DashWinddownTime;
    public float DashVelocity => DashDistance / DashMovementTime;
    [Header("Double Jump")]
    public float DoubleJumpVelocity;
    public float DoubleJumpMinTime;
    public float DoubleJumpWindupTime;
    public float DoubleJumpHoverVelocity;

}