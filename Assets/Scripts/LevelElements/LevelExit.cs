using UnityEngine;
using Zenject;

public class LevelExit : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() is PlayerController controller)
        {
            //levelManager.LevelExitReached();
            Debug.Log("Level exit reached");
        }
    }
}
