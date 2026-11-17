using UnityEngine;
using Game.Player;
namespace Game.Hazards
{
    [RequireComponent(typeof(Collider2D))]
    public class BaseHazard : MonoBehaviour, IPlayerInteractable
    {
        public float colliderReduction;
        public enum HazardType
        {
            Doom,
            DoubleJump,
            Shield,
            Dash,
        }

        public HazardType Type;

        public virtual void OnPlayerOverlap(PlayerController playerController)
        {
            playerController.MarkAsDead();
        }

    }
}