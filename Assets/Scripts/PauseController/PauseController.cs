using System;
using UnityEngine;
using Zenject;
[RequireComponent(typeof(TimeContext))]
public class PauseController : MonoBehaviour
{
    [Inject]
    ApplicationStateManager _appStateManager;

    [Inject]
    UIInputHandler _uiInputHandler;


    TimeContext _timeContext;

    public SimpleMenu PauseMenuPrefab;

    private SimpleMenu _pauseMenuInstance;

    private bool _paused;

    public Canvas Canvas;

    void Start()
    {
        _timeContext = GetComponent<TimeContext>();
        _uiInputHandler.OnMenuButtonPressed += MenuButtonPressed;
    }

    void OnDestroy()
    {
        _uiInputHandler.OnMenuButtonPressed -= MenuButtonPressed;
    }

    private void MenuButtonPressed()
    {
        TryChangePause();
    }

    private void TryChangePause()
    {
        if (_paused) // If paused, try to unpause
        {
            if (_pauseMenuInstance != null)
            {
                _pauseMenuInstance.CloseMenu();
            }
        }
        else // try to pause
        {
            _paused = true;
            _timeContext.Paused = _paused;
            _appStateManager.gameInputHandler.enabled = false;
            if (_pauseMenuInstance != null)
            {
                Destroy(_pauseMenuInstance.gameObject);
            }
            _pauseMenuInstance = Instantiate(PauseMenuPrefab, Canvas.transform);
            _pauseMenuInstance.OnMenuClosed += OnPauseMenuClosed;
            _pauseMenuInstance.SetInputHandler(_uiInputHandler);


        }
    }

    private void OnPauseMenuClosed()
    {
        _paused = false;
        _timeContext.Paused = _paused;
        _appStateManager.gameInputHandler.enabled = true;
    }
}
