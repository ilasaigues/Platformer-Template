using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int TotalLives;
    public int RemainingLives;

    public RespawnTrigger HardRespawnTrigger;
    public RespawnTrigger CurrentRespawnTrigger;
    public PlayerController PlayerController;

    public PlayerAbilityQueue PlayerAbilityQueue = new();


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
    }


}
