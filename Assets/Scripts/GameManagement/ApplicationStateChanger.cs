using System;
using UnityEngine;
using Zenject;

public class ApplicationStateChanger : MonoBehaviour
{

    [Inject]
    ApplicationStateManager _appStateManager;

    public enum ApplicationState
    {
        MainMenu,
        LoadingScreen,
        Game,
    }

    public ApplicationState TargetState;

    void Start()
    {
        switch (TargetState)
        {
            case ApplicationState.MainMenu:
                _appStateManager.SetState<ApplicationStateManager.MainMenuState>();
                break;
            case ApplicationState.LoadingScreen:
                _appStateManager.SetState<ApplicationStateManager.LoadingGameState>();
                break;
            case ApplicationState.Game:
                _appStateManager.SetState<ApplicationStateManager.PlayingGameState>();
                break;
        }
    }

}
