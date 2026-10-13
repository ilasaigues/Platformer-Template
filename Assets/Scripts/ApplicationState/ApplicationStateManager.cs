using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ApplicationStateManager : MonoBehaviour
{
    public List<BaseApplicationState> States = new();

    private BaseApplicationState _currentState;
    public BaseApplicationState CurrentState
    {
        get { return _currentState; }
        set
        {
            if (_currentState != value)
            {
                _currentState = value;
                OnStateChanged();
            }
        }
    }

    public event Action OnStateChanged = delegate { };

    void Start()
    {
        InitializeStates();
        CurrentState = GetState<RootMenuState>();
    }

    void Update()
    {
        CurrentState.DoUpdate();
    }

    T GetState<T>() where T : BaseApplicationState
    {
        return States.FirstOrDefault(s => s is T) as T;
    }

    void InitializeStates()
    {
        // 1. Get the base class
        Type baseType = typeof(BaseApplicationState);

        // 2. Scan the assembly for types that inherit from the base class
        States = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => baseType.IsAssignableFrom(t)    // Only subclass or implements the interface
                        && !t.IsInterface               // Exclude interfaces
                        && !t.IsAbstract)               // Exclude abstract classes
            .Select(t => (BaseApplicationState)Activator.CreateInstance(t)) // Create an instance
            .ToList();
    }
}
