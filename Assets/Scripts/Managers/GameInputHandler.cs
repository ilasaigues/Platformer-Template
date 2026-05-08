using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
public class GameInputHandler : MonoBehaviour, GameInputs.IPlayerActions
{
    // don't listen to input events while paused

    public GameInputs GameInputs;

    public InputButtonWrapper JumpButton;
    public InputButtonWrapper RockButton;
    public InputButtonWrapper DashButton;

    List<InputButtonWrapper> _buttons = new();
    public InputAxisWrapper MoveAxis;
    [Inject]
    TimeContext _timeContext;

    private Dictionary<Action<InputActionPhase>, InputActionPhase> _pauseQueue = new();


    void Awake()
    {
        MoveAxis = new InputAxisWrapper();
        JumpButton = GetNewButton();
        RockButton = GetNewButton();
        DashButton = GetNewButton();
    }

    void Start()
    {
        _timeContext.CreateContextModules(gameObject);
    }


    InputButtonWrapper GetNewButton()
    {
        var wrapper = new InputButtonWrapper(_timeContext);
        _buttons.Add(wrapper);
        return wrapper;
    }

    void OnEnable()
    {
        if (GameInputs == null) GameInputs = new GameInputs();

        GameInputs.Player.SetCallbacks(this);
        GameInputs.Player.Enable();
        _timeContext.ContextTimescale.OnValueChanged += TimeContextValueChanged;
    }
    void OnDisable()
    {
        GameInputs.Player.Disable();
        _timeContext.ContextTimescale.OnValueChanged -= TimeContextValueChanged;

    }

    void TimeContextValueChanged(float newValue)
    {

        if (newValue != 0)
        {
            OnUnpause();
            Debug.Log("Unpausing: " + newValue);
        }
    }


    void OnUnpause()
    {
        var keys = new List<Action<InputActionPhase>>(_pauseQueue.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            var function = keys[i];
            if (function != null)
            {
                var phase = _pauseQueue[function];
                function.Invoke(phase);
                Debug.Log("Execute: " + function + " | " + phase);
            }
        }

        _pauseQueue.Clear();
    }

    public void EnqueueButtonDuringPause(Action<InputActionPhase> function, InputActionPhase phase)
    {
        _pauseQueue[function] = phase;
        Debug.Log("Enqueue: " + function + " | " + phase);
    }

    public event Action<Vector2> MoveEvent = delegate { };
    void Update()
    {
        if (_timeContext.Paused) return;
        _buttons.ForEach(button => button.Update());
    }

    void LateUpdate()
    {
        if (_timeContext.Paused) return;
        _buttons.ForEach(button => button.LateUpdate());
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (_timeContext.Paused)
        {
            EnqueueButtonDuringPause(JumpButton.OnInputEvent, context.phase);
            return;
        }
        JumpButton.OnInputEvent(context.phase);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (_timeContext.Paused)
        {
            return;
        }
        MoveAxis.OnInputEvent(context.ReadValue<Vector2>());
    }



    public void OnRock(InputAction.CallbackContext context)
    {
        if (_timeContext.Paused)
        {
            EnqueueButtonDuringPause(RockButton.OnInputEvent, context.phase);
            return;
        }
        RockButton.OnInputEvent(context.phase);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (_timeContext.Paused)
        {
            EnqueueButtonDuringPause(DashButton.OnInputEvent, context.phase);
            return;
        }
        DashButton.OnInputEvent(context.phase);
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (_timeContext.Paused)
            {
                _timeContext.ContextTimescale.Value = 1;
            }
            else
            {
                _timeContext.ContextTimescale.Value = 0;
            }
        }
    }
}
