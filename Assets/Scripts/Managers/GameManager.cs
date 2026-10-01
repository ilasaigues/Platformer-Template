using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LDtkUnity;
using Unity.Cinemachine;
using Unity.VisualScripting;
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
        HardRespawnTrigger = Levels[level].GetComponentsInChildren<RespawnTrigger>().FirstOrDefault(rt => rt.respawnType == RespawnType.Hard);
        if (HardRespawnTrigger == null)
        {
            HardRespawnTrigger = Levels[level].GetComponentInChildren<LevelEntry>().AddComponent<RespawnTrigger>();
            HardRespawnTrigger.respawnType = RespawnType.Hard;
        }
        LevelManager.CurrentLevel = level;
        //SetCameraBounds(Levels[level]);
    }

    public void SetWorld(int world)
    {
        LevelManager.CurrentWorldIndex = world;
    }

    public async void LevelEndReached()
    {
        SetLevel(LevelManager.CurrentLevel + 1);
        PlayerController.InputHandler.BlockInputs(true);
        PlayerController.ChangingLevel = true;
        // await level out animation
        await Task.Delay(300); // TODO: REMOVE HARDCODED VALUE

        var playerPos = PlayerController.transform.position;
        var targetPos = HardRespawnTrigger.RespawnPosition;


        var transitionTime = 3; // TODO: REMOVE HARDCODED VALUE

        var tween = LeanTween.move(PlayerController.gameObject, targetPos, transitionTime).setEaseInOutCubic();
        bool complete = false;
        tween.setOnComplete(_ => complete = true);

        while (!complete)
        {
            await Task.Delay(100);
        }


        PlayerController.ChangingLevel = false;
        await Task.Delay(300); // TODO: REMOVE HARDCODED VALUE
        PlayerController.InputHandler.BlockInputs(false);
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
