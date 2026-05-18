using System.Threading.Tasks;
using UnityEngine;

public abstract class SceneTransition : MonoBehaviour
{
    public float LoadingMinSeconds = 1;

    public abstract Task EnterAsync(Canvas target);
    public abstract Task ExitAsync(Canvas target);
}
