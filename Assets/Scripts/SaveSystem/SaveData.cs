using System;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public event Action OnDataChanged = delegate { };

    public int LastReachedWorldIndex { get; private set; }
    public int LastReachedLevelIndex { get; private set; }

    public int Deaths { get; private set; }

    public void SetLastReached(int worldIndex, int levelIndex)
    {
        LastReachedWorldIndex = worldIndex;
        LastReachedLevelIndex = levelIndex;
        OnDataChanged();
    }

    public void AddDeath()
    {
        Deaths++;
        OnDataChanged();
    }

}
