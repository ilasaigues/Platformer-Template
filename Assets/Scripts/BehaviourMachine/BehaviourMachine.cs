using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace Game.BehaviourMachine
{
    public abstract class BehaviourMachine<T> : MonoBehaviour
    {

        Dictionary<Type, BaseBehaviour<T>> _allBehaviours = new();
        [Inject]
        private TimeContext _timeContext;

        BaseBehaviour<T> _currentBehaviour;
        public string GetBehaviourName => _currentBehaviour?.GetType().Name;
        public Type GetBehaviourType => _currentBehaviour?.GetType();

        void Start()
        {
        }

        public void AddBehaviour(BaseBehaviour<T> newBehaviour)
        {
            if (!_allBehaviours.ContainsKey(newBehaviour.GetType()))
            {
                _allBehaviours.Add(newBehaviour.GetType(), newBehaviour);
            }
        }

        void Update()
        {
            CheckBehaviourChange();
            _currentBehaviour.Update(_timeContext.DeltaTime);
        }

        void FixedUpdate()
        {
            _currentBehaviour.FixedUpdate(_timeContext.FixedDeltaTime);
        }

        private void CheckBehaviourChange()
        {
            if (_currentBehaviour.VerifyBehaviour() is BehaviourChangeRequest changeRequest)
            {
                ChangeBehaviour(changeRequest.NewBehaviourType);
            }
        }

        public U GetBehaviour<U>() where U : BaseBehaviour
        {
            if (_allBehaviours.TryGetValue(typeof(U), out BaseBehaviour<T> behaviour))
            {
                return behaviour as U;
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
                //Debug.Log("Entering state: " + behaviourType.ToString());
                _currentBehaviour?.Exit();
                _currentBehaviour = nextBehaviour;
                _currentBehaviour.Enter();
                CheckBehaviourChange();
            }
            _watchdog = 0;
        }

    }
}