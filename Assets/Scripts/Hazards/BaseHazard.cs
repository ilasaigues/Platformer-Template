using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BaseHazard : MonoBehaviour
{
    public enum HazardType
    {
        Doom,
        DoubleJump,
        Shield,
        Dash,
    }

    public HazardType Type;

}
