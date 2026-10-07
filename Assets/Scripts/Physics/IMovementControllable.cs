using System.Runtime.InteropServices;
using UnityEngine;

public interface IMovementControllable
{

    float FallVelocityCap { get; }
    float LedgeCorrectionUp { get; }
    float LedgeCorrectionDown { get; }
    float CeilingCorrection { get; }
    Bounds Bounds { get; }
    Collider2D MainCollider { get; }
}
