using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/Game Settings")]
public class SettingsData : ScriptableObject
{

    [Range(0, 10)] public int MasterVolume;
    [Range(0, 10)] public int MusicVolume;
    [Range(0, 10)] public int SFXVolume;
    public FullScreenMode WindowMode;


}
