using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputHandler : MonoBehaviour, GameInputs.IUIActions
{

    public event Action OnMenuButtonPressed = delegate { };

    public GameInputs GameInputs;

    void OnEnable()
    {
        if (GameInputs == null) GameInputs = new GameInputs();

        GameInputs.UI.SetCallbacks(this);
        GameInputs.UI.Enable();
    }
    void OnDisable()
    {
        GameInputs.Player.Disable();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnMenuButtonPressed();
        }
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }


}
