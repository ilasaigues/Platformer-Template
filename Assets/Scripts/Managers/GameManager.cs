using System.Collections.Generic;
using System.Linq;
using LDtkUnity;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject]
    public LevelManager LevelManager;

    [Inject]
    public SceneTransitionManager SceneTransitionManager;
    public int RemainingLives;

    public RespawnTrigger HardRespawnTrigger;
    public RespawnTrigger CurrentRespawnTrigger;
    public PlayerController PlayerController;

    public CinemachineCamera cinemachineCamera;

    public PlayerAbilityQueue PlayerAbilityQueue = new();

    public List<LDtkComponentLevel> Levels = new();

    private CinemachineConfiner2D cameraConfiner;


    public RespawnTrigger DieAndGetRespawn()
    {
        if (RemainingLives > 0)
        {
            RemainingLives--;
            return CurrentRespawnTrigger;
        }
        else
        {
            return HardRespawnTrigger;
        }
    }
    public RespawnTrigger GetRespawn()
    {
        if (RemainingLives > 0)
        {
            return CurrentRespawnTrigger;
        }
        else
        {
            return HardRespawnTrigger;
        }
    }
    public void GainAbility(IPlayerAbilityBehaviour ability)
    {
        PlayerAbilityQueue.AddAbility(ability);
    }


    void Start()
    {
        RemainingLives = LevelManager.CurrentWorldData.MaxLives;
        PlayerAbilityQueue.MaxAbilityStack = 1;
        cameraConfiner = cinemachineCamera.GetComponent<CinemachineConfiner2D>();
        SetLevel(LevelManager.CurrentLevel);
    }

    public void SetLevel(int level)
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            if (i == level - 1 || i == level || i == level + 1)
            {
                Levels[i].gameObject.SetActive(true);
            }
            else
            {
                Levels[i].gameObject.SetActive(false);
            }
        }
        HardRespawnTrigger = Levels[level].GetComponentsInChildren<RespawnTrigger>().First(rt => rt.respawnType == RespawnType.Hard);
        LevelManager.CurrentLevel = level;
        SetCameraBounds(Levels[level]);
    }

    public void SetWorld(int world)
    {
        LevelManager.CurrentWorldIndex = world;
    }

    public void DoHardRespawn()
    {
        SceneTransitionManager.TransitionToScene(LevelManager.Worlds[LevelManager.CurrentWorldIndex].SceneReference.SceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void SetCameraBounds(LDtkComponentLevel level)
    {
        Debug.Log(level);
        var referenceCollider = level.GetComponentsInChildren<LDtkComponentEntity>().First(e => e.Identifier == "Camera_bound").GetComponent<BoxCollider2D>();
        ((BoxCollider2D)cameraConfiner.BoundingShape2D).size = referenceCollider.size;
        ((BoxCollider2D)cameraConfiner.BoundingShape2D).offset = referenceCollider.offset;
        cameraConfiner.BoundingShape2D.transform.position = referenceCollider.transform.position;
        cameraConfiner.InvalidateBoundingShapeCache();
    }

}
