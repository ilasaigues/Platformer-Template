using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourMachine : MonoBehaviour
{

    Dictionary<Type, BaseBehaviour> _allBehaviours = new();

    BaseBehaviour _currentBehaviour;
    public string GetBehaviourName => _currentBehaviour?.GetType().Name;
    public Type GetBehaviourType => _currentBehaviour?.GetType();


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
        if (_currentBehaviour.VerifyBehaviour() is BehaviourChangeRequest changeRequest)
        {
            ChangeBehaviour(changeRequest.NewBehaviourType);
        }
    }

    public T GetBehaviour<T>() where T : BaseBehaviour
    {
        if (_allBehaviours.TryGetValue(typeof(T), out BaseBehaviour behaviour))
        {
            return behaviour as T;
        }
        return null;
    }

    private int _watchdog = 0;
    private Queue<string> _lastBehaviours = new();

    public void ChangeBehaviour(Type behaviourType)
    {

        if (_currentBehaviour?.GetType() != behaviourType && _allBehaviours.TryGetValue(behaviourType, out var nextBehaviour))
        {
            if (_watchdog > 100)
            {
                throw new Exception($"WATCHDOG EXCEEDED: {string.Join(" > ", _lastBehaviours)}");
            }
            _watchdog++;

            _lastBehaviours.Enqueue(behaviourType.ToString());
            if (_lastBehaviours.Count > 5)
            {
                _lastBehaviours.Dequeue();
            }
            Debug.Log("Entering state: " + behaviourType.ToString());
            _currentBehaviour?.Exit();
            _currentBehaviour = nextBehaviour;
            _currentBehaviour.Enter();
            CheckBehaviourChange();
        }
        _watchdog = 0;
    }

}
