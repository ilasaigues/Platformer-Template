using UnityEngine;
using FMODUnity;
using Zenject;

public class PlayMusicOnLoad : MonoBehaviour
{
    [Inject]
    AudioManager _audioManager;
    public EventReference MusicEventReference;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioManager.ChangeBGM(MusicEventReference);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
