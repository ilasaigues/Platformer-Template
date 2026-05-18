using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Canvas))]
public class SceneTransitionManager : MonoBehaviour
{
    private Canvas SceneTransitionCanvas;
    public SceneTransition StandardTransition;

    void Start()
    {
        SceneTransitionCanvas = GetComponent<Canvas>();
        SceneTransitionCanvas.enabled = false;
    }


    public async Task TransitionToScene(int sceneIndex, LoadSceneMode loadSceneMode, SceneTransition transition = null)
    {
        if (transition == null) transition = StandardTransition;
        var transitionInstance = Instantiate(transition, SceneTransitionCanvas.transform);
        var loadTask = SceneManager.LoadSceneAsync(sceneIndex, loadSceneMode);
        loadTask.allowSceneActivation = false;
        //enable the transition canvas
        SceneTransitionCanvas.enabled = true;
        //wait for the first half of the transition to happen
        await transitionInstance.EnterAsync(SceneTransitionCanvas);
        //allow for the new scene to activate when ready
        loadTask.allowSceneActivation = true;
        //await for the scene loading to happen
        await Task.Delay((int)(transitionInstance.LoadingMinSeconds * 1000));
        await loadTask;
        //wait for the second half of the transition to happen
        await transitionInstance.ExitAsync(SceneTransitionCanvas);
        //disable the transition canvas
        SceneTransitionCanvas.enabled = false;
    }
}
