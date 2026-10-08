using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelManager : MonoBehaviour
{
    [Serializable]
    public class WorldData
    {
        public int MaxLives;
        [StaticInstances]
        public SceneReference SceneReference;
    }

    public List<WorldData> Worlds = new();
    public IntReference CurrentWorldIndex;
    public WorldData CurrentWorldData => Worlds[CurrentWorldIndex.Value];
    public IntReference CurrentLevelIndex;

    public void SetLevelIndex(int levelIndex)
    {
        CurrentLevelIndex.Value = levelIndex;
    }

    public void SetWorldIndex(int worldIndex, int levelIndex = 0)
    {
        CurrentLevelIndex.Value = levelIndex;
        CurrentWorldIndex.Value = worldIndex;
    }
}
