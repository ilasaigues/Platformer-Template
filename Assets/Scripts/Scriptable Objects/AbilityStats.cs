using UnityEngine;

[CreateAssetMenu(fileName = "AbilityStats", menuName = "Scriptable Objects/AbilityStats")]
public class AbilityStats : ScriptableObject
{
    [Header("Rock")]
    public float RockGravity;
    public float RockBreakSpeed;
    [Header("Dash")]
    public float DashWindupTime;
    public float DashMovementTime;
    public float DashWinddownTime;

    public float DashDistance;
    public float DashCooldown;
    public float TotalDashTime => DashWinddownTime + DashMovementTime + DashWinddownTime;
    public float DashVelocity => DashDistance / DashMovementTime;
}