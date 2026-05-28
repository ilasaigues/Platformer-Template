using System.Collections.Generic;
using System.Linq;
using LDtkUnity;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int TotalLives;
    public int RemainingLives;

    public RespawnTrigger HardRespawnTrigger;
    public RespawnTrigger CurrentRespawnTrigger;
    public PlayerController PlayerController;

    public CinemachineCamera cinemachineCamera;

    public PlayerAbilityQueue PlayerAbilityQueue = new();

    public List<LDtkComponentLevel> Levels = new();

    public int CurrentLevel = 0;

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
        RemainingLives = TotalLives;
        PlayerAbilityQueue.MaxAbilityStack = 1;
        var confiner = cinemachineCamera.GetComponent<CinemachineConfiner2D>();
        confiner.BoundingShape2D = Levels[CurrentLevel].GetComponentsInChildren<LDtkComponentEntity>().First(e => e.Identifier == "Camera_bound").GetComponent<Collider2D>();
        confiner.InvalidateBoundingShapeCache();
    }


}
