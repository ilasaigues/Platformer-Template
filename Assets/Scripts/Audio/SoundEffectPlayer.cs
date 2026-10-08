using UnityEngine;
using FMODUnity;

public class SoundEffectPlayer : MonoBehaviour
{
    public void PlaySFX(EventReference SFXreference)
    {
        var instance = RuntimeManager.CreateInstance(SFXreference);
        instance.start();
    }
    public void PlaySFXReference(SFXEvent SFXEvent)
    {
        PlaySFX(SFXEvent.eventReference);
    }
}
 
