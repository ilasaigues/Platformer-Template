using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    [Inject]
    public GameInputHandler InputHandler;


    public enum AbilityType
    {
        DoubleJump,
        Dash,
        Shield,
    }

    public event Action<AbilityType> AbilityTypeChanged;

    [NonSerialized]
    public bool ChangingLevel = false;

    public int RemainingLives;

    public RespawnTrigger HardRespawnTrigger;
    public RespawnTrigger CurrentRespawnTrigger;

    public CinemachineCamera cinemachineCamera;


    public List<LDtkComponentLevel> Levels = new();

    private CinemachineConfiner2D cameraConfiner;


    public (Vector2 respawnPosition, RespawnType respawnType) DieAndGetRespawn()
    {
        if (RemainingLives > 0)
        {
            RemainingLives--;
            Debug.Log("soft death");
            return (CurrentRespawnTrigger.RespawnPosition, CurrentRespawnTrigger.respawnType);
        }
        else
        {
            Debug.Log("hard death");
            return (HardRespawnTrigger.RespawnPosition, HardRespawnTrigger.respawnType);
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

    public Vector3 GetRespawnPosition()
    {
        return GetRespawn().RespawnPosition;
    }

    public void GainAbility(AbilityType abilityType)
    {
        AbilityTypeChanged(abilityType);
    }


    void Start()
    {

        //Get all levels in this world, named "WORLD_X_Y", where X is the world, and Y is the level
        Levels = FindObjectsByType<LDtkComponentLevel>(FindObjectsSortMode.None).OrderBy(l => Convert.ToInt32(l.name.Split('_').Last())).ToList();

        RemainingLives = LevelManager.CurrentWorldData.MaxLives;
        //PlayerAbilityQueue.MaxAbilityStack = 1;
        cameraConfiner = cinemachineCamera.GetComponent<CinemachineConfiner2D>();
        SetLevel(LevelManager.CurrentLevel);

        foreach (var levelExit in FindObjectsByType<LevelExit>(FindObjectsSortMode.None))
        {
            levelExit.OnExitTriggered += async traveller =>
            {
                await LevelEndReached(traveller);
            };
        }

        foreach (var levelExit in FindObjectsByType<RespawnTrigger>(FindObjectsSortMode.None))
        {
            levelExit.OnRespawnTriggered += respawn =>
            {
                switch (respawn.respawnType)
                {
                    case RespawnType.Soft:
                        CurrentRespawnTrigger = respawn;
                        return;
                    case RespawnType.Hard:
                        HardRespawnTrigger = respawn;
                        CurrentRespawnTrigger ??= respawn;
                        return;
                }
            };
        }

    }

    public void SetLevel(int level)
    {
        /*for (int i = 0; i < Levels.Count; i++)
        {
            if (i == level - 1 || i == level || i == level + 1)
            {
                Levels[i].gameObject.SetActive(true);
            }
            else
            {
                Levels[i].gameObject.SetActive(false);
            }
        }*/
        HardRespawnTrigger = Levels[level].GetComponentsInChildren<RespawnTrigger>().FirstOrDefault(rt => rt.respawnType == RespawnType.Hard);

        LevelManager.CurrentLevel = level;
        //SetCameraBounds(Levels[level]);
    }

    public void SetWorld(int world)
    {
        LevelManager.CurrentWorldIndex = world;
    }

    public async Task LevelEndReached(LevelTraversalComponent traveller)
    {
        SetLevel(LevelManager.CurrentLevel + 1);
        InputHandler.BlockInputs(true);
        ChangingLevel = true;
        // await level out animation
        int startDelay = (int)(traveller.BeforeTravelDelay * 1000);
        await Task.Delay(startDelay);

        var playerPos = traveller.transform.position;
        var targetPos = HardRespawnTrigger.RespawnPosition;


        var transitionTime = traveller.TraveltransitionDuration;

        var tween = LeanTween.move(traveller.gameObject, targetPos, transitionTime).setEaseInOutCubic();
        bool complete = false;
        tween.setOnComplete(_ => complete = true);

        while (!complete)
        {
            await Task.Delay(100);
        }


        ChangingLevel = false;
        var endDelay = (int)(traveller.AfterTravelDelay * 1000);

        await Task.Delay(endDelay);
        InputHandler.BlockInputs(false);
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
