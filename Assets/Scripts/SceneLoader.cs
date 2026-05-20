using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneLoader : MonoBehaviour
{

    [Inject]
    SceneTransitionManager SceneTransitionManager;


    [StaticInstances]
    public SceneReference SceneReference;
    public bool LoadAdditive;


    public void LoadScene()
    {
        if (LoadAdditive)
        {
            SceneManager.LoadScene(SceneReference.SceneName, LoadSceneMode.Additive);
        }
        else
        {
            SceneTransitionManager.TransitionToScene(SceneReference.SceneName, LoadSceneMode.Single);
        }
    }


}
