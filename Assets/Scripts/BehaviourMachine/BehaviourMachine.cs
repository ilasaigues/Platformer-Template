using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourMachine : MonoBehaviour
{

    Dictionary<Type, BaseBehaviour> _allBehaviours = new();

    BaseBehaviour _currentBehaviour;
    public string GetBehaviourName => _currentBehaviour?.GetType().Name;


    public void AddBehaviour(BaseBehaviour newBehaviour)
    {
        if (!_allBehaviours.ContainsKey(newBehaviour.GetType()))
        {
            _allBehaviours.Add(newBehaviour.GetType(), newBehaviour);
        }
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
            Debug.Log("Entering state: " + behaviourType.ToString());
            _currentBehaviour?.Exit();
            _currentBehaviour = nextBehaviour;
            _currentBehaviour.Enter();
            CheckBehaviourChange();
        }
    }

}
