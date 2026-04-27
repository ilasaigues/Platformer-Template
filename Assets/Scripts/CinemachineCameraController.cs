using Unity.Cinemachine;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CinemachinePositionComposer))]
public class CinemachineCameraController : MonoBehaviour
{
    [SerializeField]
    private MovementController _targetMovementController;

    private CinemachinePositionComposer _posComposer;

    public Vector2 DeadzoneSize;

    [Inject]
    GameManager _gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _posComposer = GetComponent<CinemachinePositionComposer>();
        _targetMovementController ??= _gameManager.PlayerController.MovementController;
        GetComponent<CinemachineCamera>().Target.TrackingTarget = _targetMovementController.transform;
    }

    // Update is called once per frame
    void Update()
    {
        var deadzoneRect = _posComposer.Composition.DeadZoneRect;
        if (_targetMovementController.Grounded)
        {
            deadzoneRect.size = new Vector2(DeadzoneSize.x, 0);
        }
        else
        {
            deadzoneRect.size = DeadzoneSize;
        }
        _posComposer.Composition.DeadZoneRect = deadzoneRect;
        _posComposer.Composition.ScreenPosition = Vector2.zero;
    }
}
