using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class PowerupContainer : MonoBehaviour
{
    [SerializeField]
    private PowerupDisplay _powerupDisplayPrefab;
    [SerializeField]
    private Transform _container;

    private List<PowerupDisplay> _displays = new();

    [Inject]
    GameManager _gameManager;

    void Start()
    {
        _gameManager.PlayerAbilityQueue.OnPlayerAbilityEnqueued += PlayerAbilitiesChanged;
        _gameManager.PlayerAbilityQueue.OnPlayerAbilityDequeued += PlayerAbilitiesChanged;
        UpdateView();
    }

    void PlayerAbilitiesChanged(IPlayerAbilityBehaviour _)
    {
        UpdateView();
    }

    void UpdateView()
    {
        while (_displays.Count < _gameManager.PlayerAbilityQueue.MaxAbilityStack)
        {
            _displays.Add(Instantiate(_powerupDisplayPrefab, _container));
        }

        while (_displays.Count > _gameManager.PlayerAbilityQueue.MaxAbilityStack)
        {
            Destroy(_displays.Last().gameObject);
            _displays.RemoveAt(_displays.Count - 1);
        }

        for (int i = 0; i < _displays.Count; i++)
        {
            if (i < _gameManager.PlayerAbilityQueue.AbilityQueue.Count)
            {
                Debug.Log(_gameManager.PlayerAbilityQueue.AbilityQueue.ToList()[i].UIAnimation.name);
                _displays[i].Animator.Play(_gameManager.PlayerAbilityQueue.AbilityQueue.ToList()[i].UIAnimation.name);
            }
            else
            {
                _displays[i].Animator.Play("Empty");
            }
        }
    }
}
