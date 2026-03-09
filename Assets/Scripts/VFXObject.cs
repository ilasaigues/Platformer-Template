
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animation))]
public class VFXObject : MonoBehaviour
{
    public static VFXObject NewInstance(string name)
    {
        var go = new GameObject(name);
        go.AddComponent<SpriteRenderer>();
        go.AddComponent<Animation>();
        return go.AddComponent<VFXObject>();
    }

    private SpriteRenderer _spriteRenderer;
    private Animation _animation;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sortingOrder = 1;
        _animation = GetComponent<Animation>();
    }

    public void SetDataAndPlay(AnimationClip animClip, Vector2 position, bool flipX)
    {
        transform.position = position;
        _spriteRenderer.flipX = flipX;
        _animation.clip = animClip;
        _animation.AddClip(animClip, animClip.name);
        _animation.Play(animClip.name);

        //DisableAfterDelay(_animation.clip.length);
    }

    async void DisableAfterDelay(float time)
    {
        await Task.Delay((int)(time * 1000));
        gameObject.SetActive(false);
    }
}