
using System;

public interface IPlayerAbilityBehaviour
{
    public bool Enabled { get; set; }
    public bool OnCooldown { get; }
    public float TimeLastUsed { get; set; }

}