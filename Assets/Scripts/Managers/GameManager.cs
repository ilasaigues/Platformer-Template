using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int TotalLives;
    public int RemainingLives;

    public RespawnTrigger HardRespawnTrigger;
    public RespawnTrigger CurrentRespawnTrigger;
    public PlayerController PlayerController;

    public RespawnTrigger DieAndGetRespawn()
    {
        if (RemainingLives > 0)
        {
            RemainingLives--;
            return CurrentRespawnTrigger;
        }
        else
        {
            RemainingLives = TotalLives;
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

    void Start()
    {
        RemainingLives = TotalLives;
    }
}
