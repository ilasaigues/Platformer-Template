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

    public RespawnTrigger HardRespawnTrigger;
    public RespawnTrigger CurrentRespawnTrigger;

    public CinemachineCamera cinemachineCamera;

    public event Action OnLevelChanged = delegate { };

    public List<LDtkComponentLevel> Levels = new();

    private CinemachineConfiner2D cameraConfiner;


    public Vector2 GetRespawnPosition(bool hardRespawn = false)
    {
        return GetRespawn(hardRespawn).RespawnPosition;
    }
    public RespawnTrigger GetRespawn(bool hardRespawn = false)
    {
        if (hardRespawn)
        {
            return HardRespawnTrigger;
        }
        else
        {
            return CurrentRespawnTrigger;
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

        cameraConfiner = cinemachineCamera.GetComponent<CinemachineConfiner2D>();
        SetLevel(LevelManager.CurrentLevelIndex);

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

        SetHardRespawn();
        var worldExit = FindFirstObjectByType<WorldExit>();
        worldExit.OnWorldExitTriggered += WorldExitTriggered;
    }

    private void WorldExitTriggered(LevelTraversalComponent component)
    {
        SetWorld(LevelManager.CurrentWorldIndex + 1);
        SceneTransitionManager.TransitionToScene(LevelManager.Worlds[LevelManager.CurrentWorldIndex].SceneReference.SceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    void SetHardRespawn()
    {
        HardRespawnTrigger = Levels[LevelManager.CurrentLevelIndex].GetComponentsInChildren<RespawnTrigger>().FirstOrDefault(rt => rt.respawnType == RespawnType.Hard);

    }

    public void SetLevel(int level)
    {
        LevelManager.SetLevelIndex(level);
        SetHardRespawn();
        OnLevelChanged();
    }


    public void SetWorld(int world)
    {
        LevelManager.SetWorldIndex(world);
    }

    public void SetWorldAndLevel(int world, int level)
    {
        LevelManager.SetWorldIndex(world, level);
    }

    public async Task LevelEndReached(LevelTraversalComponent traveller)
    {
        SetLevel(LevelManager.CurrentLevelIndex + 1);
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
