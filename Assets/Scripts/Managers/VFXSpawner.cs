using System;
using UnityEngine;
using Zenject;

public class VFXSpawner : MonoBehaviour
{

    public VFXList VFXList;
    [Inject]
    VFXObject.Factory VFXFactory;
    public VFXObject VFXObjectPrefab;

    // Create pool
    public Pooler<VFXObject> VFXPool;

    // create function to get from pool

    void Start()
    {
        VFXPool = new(
            () =>
            {
                return VFXFactory.Create();
            },
            preparing =>
            {
                preparing.gameObject.SetActive(true);
                preparing.OnAnimationEnd.AddListener(OnVFXAnimationEnd);

            },
            released =>
            {
                released.OnAnimationEnd.RemoveListener(OnVFXAnimationEnd);
                released.GetComponent<SpriteRenderer>().sprite = null;
                released.gameObject.SetActive(false);
            }
        );
    }

    public void PlayFX(AnimationClip animClip, Vector2 position, int order, bool flipX = false)
    {
        // get an fx object from pool
        var vfxObject = VFXPool.GetElement();

        // populate with variables, set position and play
        vfxObject.SetDataAndPlay(animClip, position, order, flipX);
    }

    private void OnVFXAnimationEnd(VFXObject released)
    {
        VFXPool.Release(released);
    }

}
