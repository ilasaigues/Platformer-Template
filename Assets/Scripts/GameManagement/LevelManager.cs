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
    public int CurrentWorldIndex;
    public WorldData CurrentWorldData => Worlds[CurrentWorldIndex];
    public int CurrentLevel;
}
