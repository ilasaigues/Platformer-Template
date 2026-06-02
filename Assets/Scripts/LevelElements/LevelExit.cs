using UnityEngine;
using Zenject;

public class LevelExit : MonoBehaviour
{
    [Inject]
    private GameManager gameManager;

    bool collided = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!collided && other.GetComponent<PlayerController>() is PlayerController controller)
        {
            collided = true;
            gameManager.SetLevel(gameManager.LevelManager.CurrentLevel + 1);
            gameManager.PlayerController.OverrideMovement(
                new AxisOverride(gameManager.PlayerController.InputHandler.MoveAxis, Vector2.right, 1000)
            );
        }
    }
}
