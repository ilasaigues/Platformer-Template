using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public SaveData[] SaveSlots;
    private int _currentSaveIndex = -1;
    public int CurrentSaveIndex
    {
        get { return _currentSaveIndex; }
        set
        {
            // ensure we only listen to data changes in the current save state
            if (_currentSaveIndex != -1)
            {
                CurrentSaveData.OnDataChanged -= DataChanged;
            }
            _currentSaveIndex = value;
            if (_currentSaveIndex != -1)
            {
                CurrentSaveData.OnDataChanged += DataChanged;
            }
        }
    }

    public SaveData CurrentSaveData => CurrentSaveIndex == -1 ? null : SaveSlots[CurrentSaveIndex];

    public void DataChanged()
    {
        //save the current save file if != -1
    }
}
