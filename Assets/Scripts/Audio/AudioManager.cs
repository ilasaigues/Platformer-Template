using UnityEngine;
using FMODUnity;
using System.IO;

public class AudioManager : MonoBehaviour
{

    public SettingsData SettingsData;
    string settingsFilePath => Path.Combine(Application.persistentDataPath, "settings.json");




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TryLoadChanges();
        RuntimeManager.GetBus("Bus:/").setVolume(SettingsData.MasterVolume / 10f);

        RuntimeManager.GetVCA("vca:/BGM").setVolume(SettingsData.MusicVolume / 10f);

        RuntimeManager.GetVCA("vca:/SFX").setVolume(SettingsData.SFXVolume / 10f);

    }

    void TryLoadChanges()
    {
        if (File.Exists(settingsFilePath))
        {
            string jsonString = File.ReadAllText(settingsFilePath);
            JsonUtility.FromJsonOverwrite(jsonString, SettingsData);
        }
    }

}
