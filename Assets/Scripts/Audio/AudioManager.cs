using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    
    private VCA _bgm,_sfx,_bgs;

    [Range (0,1)]
    public float BgmVolume = 1;
    [Range (0,1)]
    public float SfxVolume = 1;
    [Range (0,1)]
    public float BgsVolume = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _bgm = RuntimeManager.GetVCA("vca:/BGM");
        _sfx = RuntimeManager.GetVCA("vca:/SFX");
        _bgs = RuntimeManager.GetVCA("vca:/BGS");
    }

    // Update is called once per frame
    void Update()
    {
        _bgm.setVolume(BgmVolume);
        _sfx.setVolume(SfxVolume);
        _bgs.setVolume(BgsVolume);
    }
}
