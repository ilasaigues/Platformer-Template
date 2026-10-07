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

    PlayerController PlayerController;

    void Start()
    {
        PlayerController ??= FindFirstObjectByType<PlayerController>();
        PlayerController.AbilityQueue.OnPlayerAbilityEnqueued += PlayerAbilitiesChanged;
        PlayerController.AbilityQueue.OnPlayerAbilityDequeued += PlayerAbilitiesChanged;
        UpdateView();
    }

    void PlayerAbilitiesChanged(IPlayerAbilityBehaviour _)
    {
        UpdateView();
    }

    void UpdateView()
    {
        while (_displays.Count < PlayerController.AbilityQueue.MaxAbilityStack)
        {
            _displays.Add(Instantiate(_powerupDisplayPrefab, _container));
        }

        while (_displays.Count > PlayerController.AbilityQueue.MaxAbilityStack)
        {
            Destroy(_displays.Last().gameObject);
            _displays.RemoveAt(_displays.Count - 1);
        }

        for (int i = 0; i < _displays.Count; i++)
        {
            if (i < PlayerController.AbilityQueue.AbilityQueue.Count)
            {
                Debug.Log(PlayerController.AbilityQueue.AbilityQueue.ToList()[i].UIAnimation.name);
                _displays[i].Animator.Play(PlayerController.AbilityQueue.AbilityQueue.ToList()[i].UIAnimation.name);
            }
            else
            {
                _displays[i].Animator.Play("Empty");
            }
        }
    }
}
