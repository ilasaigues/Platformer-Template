using UnityEngine;

public class VFXSpawner : MonoBehaviour
{
    public static VFXSpawner Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<VFXSpawner>();
                DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }
    private static VFXSpawner _instance;

    public VFXList VFXList;

    // Create pool

    // create function to get from pool

    public void PlayFX(AnimationClip animClip, Vector2 position, bool flipX = false)
    {
        // get an fx object from pool
        var vfxObject = VFXObject.NewInstance(animClip.name);

        // populate with variables, set position and play
        vfxObject.SetDataAndPlay(animClip, position, flipX);
    }
}
