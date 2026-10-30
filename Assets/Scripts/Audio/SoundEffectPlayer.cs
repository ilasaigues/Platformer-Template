using UnityEngine;
using FMODUnity;
[RequireComponent(typeof(StudioEventEmitter))]

public class SoundEffectPlayer : MonoBehaviour
{
    StudioEventEmitter _eventEmitter;
    public void PlaySFX(EventReference SFXreference)
    {
        RuntimeManager.PlayOneShotAttached(SFXreference, gameObject);
    }
    public void PlaySFXReference(SFXEvent SFXEvent)
    {
        PlaySFX(SFXEvent.eventReference);
    }
}
 
