using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachinePositionComposer))]
public class CinemachineCameraController : MonoBehaviour
{
    [SerializeField]
    private MovementController _targetMovementController;

    private CinemachinePositionComposer _posComposer;

    public Vector2 DeadzoneSize;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _posComposer = GetComponent<CinemachinePositionComposer>();
    }

    // Update is called once per frame
    void Update()
    {
        _targetMovementController ??= FindFirstObjectByType<PlayerController>().MovementController;
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
