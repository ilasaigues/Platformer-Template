
using System;

public interface IPlayerAbilityBehaviour
{
    public bool Enabled { get; set; }
    public bool OnCooldown { get; }
    public DateTime TimeLastUsed { get; set; }

}