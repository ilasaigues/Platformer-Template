using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "PlayerSoundList", menuName = "Scriptable Objects/PlayerSoundList")]
public class PlayerSoundList : ScriptableObject
{
    public SFXEvent jumpSFX, stepSFX, doubleJumpSFX;
}
