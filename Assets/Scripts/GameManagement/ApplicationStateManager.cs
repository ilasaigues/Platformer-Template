using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ApplicationStateManager : MonoBehaviour
{
    [Inject]
    public GameInputHandler gameInputHandler { get; private set; }
    [Inject]
    public UIInputHandler UIInputHandler { get; private set; }

    public TimeContext GameTimeContext;


    public abstract class BaseApplicationState
    {
        protected ApplicationStateManager appStateManager;
        public BaseApplicationState(ApplicationStateManager applicationStateManager)
        {
            appStateManager = applicationStateManager;
        }
        public virtual void EnterState() { }
        public virtual void ExitState() { }
    }
    public class MainMenuState : BaseApplicationState
    {
        public MainMenuState(ApplicationStateManager applicationStateManager) : base(applicationStateManager) { }

        public override void EnterState()
        {
            appStateManager.gameInputHandler.enabled = false;
            appStateManager.UIInputHandler.enabled = true;
        }
    }
    public class LoadingGameState : BaseApplicationState
    {
        public LoadingGameState(ApplicationStateManager applicationStateManager) : base(applicationStateManager) { }

        public override void EnterState()
        {
            appStateManager.gameInputHandler.enabled = false;
            appStateManager.UIInputHandler.enabled = false;
        }
    }
    public class PlayingGameState : BaseApplicationState
    {
        public PlayingGameState(ApplicationStateManager applicationStateManager) : base(applicationStateManager) { }

        public override void EnterState()
        {
            Debug.Log("Entered game state");
            appStateManager.gameInputHandler.enabled = true;
            appStateManager.UIInputHandler.enabled = true;
        }


    }

    List<BaseApplicationState> _states;
    BaseApplicationState _currentState;


    void Start()
    {
        _states = new(){
            new PlayingGameState(this),
            new MainMenuState(this),
            new LoadingGameState(this),
        };
        SetState<MainMenuState>();
    }



    public void SetState<T>() where T : BaseApplicationState
    {
        _currentState?.ExitState();
        _currentState = _states.First(s => s is T);
        _currentState.EnterState();
    }

    public T GetState<T>() where T : BaseApplicationState
    {
        return _states.First(s => s is T) as T;
    }
    public void SetState(Type type)
    {
        if (_states.Any(s => s.GetType() == type))
        {
            _currentState?.ExitState();
            _currentState = _states.First(s => s.GetType() == type);
            _currentState.EnterState();
        }
    }

}
