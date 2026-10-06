using UnityEngine;

public interface ISqueezable
{
    Vector2 Position { get; }
    bool CanBeSqueezed { get; }
    void SetExternalVelocity(Vector2 velocity);
    void Squeeze();
    Bounds Bounds { get; }
}
