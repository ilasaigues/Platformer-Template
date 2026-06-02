using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class LevelEntry : MonoBehaviour
{
    [SerializeField]
    private GameObject EntryBlockerPrefab;

    bool exited = false;
    void OnTriggerExit2D(Collider2D other)
    {
        if (!exited && other.GetComponent<PlayerController>() is PlayerController controller)
        {
            exited = true;
            var entryBlocker = Instantiate(EntryBlockerPrefab);
            entryBlocker.transform.position = transform.position;

        }
    }
}
