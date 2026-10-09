using UnityEngine;

public class LevelTraversalComponent : MonoBehaviour
{

    public float BeforeTravelDelay = 1;
    public float TraveltransitionDuration = 3;
    public float AfterTravelDelay = 1;
    public bool IsTraveling = false;
    /*
        public async void Respawn()
        {
            var respawn = GameManager.DieAndGetRespawn();

            if (respawn.respawnType == RespawnType.Hard) // reload scene
            {
                GameManager.DoHardRespawn();
            }
            else
            {

                IsDead = false;
                var startPos = respawn.RespawnPosition;
                Debug.Log(respawn.RespawnPosition);
                Debug.DrawRay(startPos, Vector2.down * 10, Color.red, 1);
                var groundOffset = PlayerStats.DefaultColliderSize.y / 2;
                var hit = Physics2D.Raycast(startPos, Vector2.down, 10, LayerReference.TerrainLayer);
                if (hit)
                {
                    LeanTween.cancelAll();
                    MovementController.ForcePosition(hit.point + Vector2.up * groundOffset);
                }
            }

        }


        public void SetRespawn(RespawnTrigger respawn, RespawnType respawnType)
        {
            if (GameManager != null)
            {
                switch (respawnType)
                {
                    case RespawnType.Soft:
                        GameManager.CurrentRespawnTrigger = respawn;
                        return;
                    case RespawnType.Hard:
                        GameManager.HardRespawnTrigger = respawn;
                        GameManager.CurrentRespawnTrigger ??= respawn;
                        GameManager.PlayerAbilityQueue.Clear();
                        return;
                }
            }
        }
    */



}