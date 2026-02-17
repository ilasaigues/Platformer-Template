using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourMachine : MonoBehaviour
{

    Dictionary<Type, BaseBehaviour> _allBehaviours = new();

    BaseBehaviour _currentBehaviour;

    void Start()
    {
        // Create instance of all states

        // Set initial state (falling?)
    }

    void Update()
    {
        CheckBehaviourChange();
        _currentBehaviour.Update(Time.deltaTime);
    }

    void FixedUpdate()
    {
        _currentBehaviour.FixedUpdate(Time.fixedDeltaTime);
    }

    private void CheckBehaviourChange()
    {
        if (_currentBehaviour.VerifyBehaviour() is BehaviourChangeRequest changeRequest &&
            _currentBehaviour.GetType() != changeRequest.NewBehaviourType)
        {
            ChangeBehaviour(changeRequest.NewBehaviourType);
        }
    }

    public void ChangeBehaviour(Type behaviourType)
    {
        if (_allBehaviours.TryGetValue(behaviourType, out var nextBehaviour))
        {
            _currentBehaviour.Exit();
            _currentBehaviour = nextBehaviour;
            _currentBehaviour.Enter();
            CheckBehaviourChange();
        }
    }

}
