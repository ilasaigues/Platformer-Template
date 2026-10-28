using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "MenuSoundList", menuName = "Scriptable Objects/MenuSoundList")]
public class MenuSoundList : ScriptableObject
{
    public SFXEvent HoverSoundEventReference;
    public SFXEvent ConfirmSoundEventReference;
    public SFXEvent CancelMenuSoundEventReference;
    public SFXEvent PauseEnterEventReference;
    public SFXEvent PauseExitEventReference;
    public SFXEvent SFXVolumeChangeSoundEventReference;
    public SFXEvent BGMVolumeChangeSoundEventReference;


}
