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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // load existing saved data, if any. Otherwise use default save values

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


    void OnWindowModeChanged(int index)
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
        SaveChanges();
    }

    public void MasterVolumeChanged(int newVolume)
    {
        SettingsData.MasterVolume = newVolume;
        SaveChanges();
    }

    public void MusicVolumeChanged(int newVolume)
    {
        SettingsData.MusicVolume = newVolume;
        SaveChanges();
    }

    public void SFXVolumeChanged(int newVolume)
    {
        SettingsData.SFXVolume = newVolume;
        SaveChanges();
    }


    void SaveChanges()
    {

    }
}
