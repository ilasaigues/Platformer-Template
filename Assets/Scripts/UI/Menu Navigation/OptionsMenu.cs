using System.IO;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    public Slider MasterVolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider SFXVolumeSlider;
    public TMP_Dropdown WindowModeSelector;

    public SettingsData SettingsData;

    string settingsFilePath => Path.Combine(Application.persistentDataPath, "settings.json");


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // load existing saved data, if any. Otherwise use default save values
        TryLoadChanges();

        // Apply the loaded values to the sliders
        MasterVolumeSlider.value = SettingsData.MasterVolume;
        MusicVolumeSlider.value = SettingsData.MusicVolume;
        SFXVolumeSlider.value = SettingsData.SFXVolume;
        switch (SettingsData.WindowMode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                WindowModeSelector.value = 0;
                break;
            case FullScreenMode.FullScreenWindow:
                WindowModeSelector.value = 1;
                break;
            case FullScreenMode.MaximizedWindow:
            case FullScreenMode.Windowed:
                WindowModeSelector.value = 2;
                break;
        }
    }


    public void WindowModeChanged(int index)
    {
        switch (index)
        {
            case 0:
                SettingsData.WindowMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                SettingsData.WindowMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                SettingsData.WindowMode = FullScreenMode.Windowed;
                break;

        }
        Screen.fullScreenMode = SettingsData.WindowMode;
        SaveChanges();
    }

    public void MasterVolumeChanged(float newVolume)
    {
        SettingsData.MasterVolume = (int)newVolume;
        RuntimeManager.GetBus("Bus:/").setVolume(newVolume / 10f);
        SaveChanges();
    }

    public void MusicVolumeChanged(float newVolume)
    {
        SettingsData.MusicVolume = (int)newVolume;
        RuntimeManager.GetVCA("vca:/BGM").setVolume(newVolume / 10f);
        SaveChanges();
    }

    public void SFXVolumeChanged(float newVolume)
    {
        SettingsData.SFXVolume = (int)newVolume;
        RuntimeManager.GetVCA("vca:/SFX").setVolume(newVolume / 10f);
        SaveChanges();
    }


    void TryLoadChanges()
    {
        if (File.Exists(settingsFilePath))
        {
            string jsonString = File.ReadAllText(settingsFilePath);
            JsonUtility.FromJsonOverwrite(jsonString, SettingsData);
        }
    }

    void SaveChanges()
    {
        var settingsText = JsonUtility.ToJson(SettingsData);
        File.WriteAllText(settingsFilePath, settingsText);
    }
}
