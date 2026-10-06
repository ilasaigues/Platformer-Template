using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CinemachinePositionComposer))]
public class CinemachineCameraController : MonoBehaviour
{
    [SerializeField]
    private MovementController _targetMovementController;

    private CinemachinePositionComposer _posComposer;

    public Vector2 DeadzoneSize;
    public Vector2 AirOffset;
    public Vector2 GroundOffset;
    public Vector2 GroundDamping;
    public Vector2 AirDamping;

    [Inject]
    GameManager _gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _posComposer = GetComponent<CinemachinePositionComposer>();
        _targetMovementController ??= FindFirstObjectByType<PlayerController>().GetComponent<MovementController>();
        GetComponent<CinemachineCamera>().Target.TrackingTarget = _targetMovementController.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var deadzoneRect = _posComposer.Composition.DeadZoneRect;
        if (_targetMovementController.Grounded)
        {
            deadzoneRect.size = new Vector2(DeadzoneSize.x, 0);
            _posComposer.TargetOffset = GroundOffset;
            _posComposer.Lookahead.IgnoreY = true;
            _posComposer.Damping = GroundDamping;
        }
        else
        {
            deadzoneRect.size = DeadzoneSize;
            Vector2 verticalmultiplier = -Mathf.Sign(_targetMovementController.Velocity.y) * Vector2.up;
            _posComposer.TargetOffset = AirOffset*verticalmultiplier;
            _posComposer.Lookahead.IgnoreY = false;
            _posComposer.Damping = AirDamping;
        }
        _posComposer.Composition.DeadZoneRect = deadzoneRect;
        _posComposer.Composition.ScreenPosition = Vector2.zero;
    }
}
