using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;
[RequireComponent(typeof(CollisionController))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(BehaviourMachine))]
[RequireComponent(typeof(PlayerAnimator))]
public class PlayerController : MonoBehaviour, IMovementControllable, ISqueezable
{
    #region Injected
    [Inject]
    [HideInInspector]
    public GameInputHandler InputHandler;
    [Inject]
    [HideInInspector]
    public TimeContext TimeContext;
    [Inject]
    [HideInInspector]
    public VFXSpawner VFXSpawner;

    [Inject]
    private SceneTransitionManager _sceneTransitionManager;

    public GameManager GameManager;

    [Inject]
    public void Constructor(GameManager gameManager)
    {
        GameManager = gameManager;
    }


    #endregion

    public PlayerAbilityQueue AbilityQueue = new();

    public PlayerStats PlayerStats;
    public AbilityStats AbilityStats;
    public CollisionController CollisionController;
    public MovementController MovementController;
    public PlayerAnimator PlayerAnimator;

    public SpriteTrail ExternalSpriteTrail;

    public BehaviourMachine BehaviourMachine;

    public ParticleSystem DashParticles;

    private float minDeadzone = 0.125f;

    public Vector2 LastDirectionInput
    {
        get
        {
            var lastInput = InputHandler.MoveAxis.LastValue;
            if (Mathf.Abs(lastInput.x) < minDeadzone) lastInput.x = 0;
            if (Mathf.Abs(lastInput.y) < minDeadzone) lastInput.y = 0;
            return lastInput;
        }
    }
    public Vector2 LastHorizontalDirection { get; private set; }

    public int RemainingDashes = 1;

    public int Jumps = 0;

    public bool IsDead;

    public SpriteRenderer SpriteRenderer;

    public int FacingDirection => SpriteRenderer.flipX ? -1 : 1;

    public CinemachinePositionComposer CameraComposer;


    public int RemainingLives { get; private set; }

    public float FallVelocityCap => PlayerStats.fallVelocityCap;

    public float LedgeCorrectionUp => PlayerStats.ledgeCorrectionUp;

    public float LedgeCorrectionDown => PlayerStats.ledgeCorrectionDown;

    public float CeilingCorrection => PlayerStats.ceilingCorrection;

    public Bounds Bounds => MainCollider.bounds;

    public Collider2D MainCollider => CollisionController.MainCollider;

    public Vector2 Position => transform.position;

    public bool CanBeSqueezed => MovementController.CanBeSqueezed;

    void Start()
    {
        TimeContext.CreateContextModules(gameObject);
        SpriteRenderer = gameObject.GetOrAddComponent<SpriteRenderer>();
        CollisionController = gameObject.GetOrAddComponent<CollisionController>();
        MovementController = gameObject.GetOrAddComponent<MovementController>();
        PlayerAnimator = gameObject.GetOrAddComponent<PlayerAnimator>();
        BehaviourMachine = gameObject.GetOrAddComponent<BehaviourMachine>();
        BehaviourMachine.AddBehaviour(new PlayerFallingBehaviour(this));
        BehaviourMachine.AddBehaviour(new PlayerIdleBehaviour(this));
        BehaviourMachine.AddBehaviour(new PlayerGroundMoveBehaviour(this));
        BehaviourMachine.AddBehaviour(new PlayerJumpingBehaviour(this));
        BehaviourMachine.AddBehaviour(new PlayerDyingBehaviour(this));
        BehaviourMachine.AddBehaviour(new PlayerLevelTransitionBehaviour(this));
        BehaviourMachine.AddBehaviour(new PlayerRockBehaviour(this)
        {
            //Enabled = true
        });
        BehaviourMachine.AddBehaviour(new PlayerDashBehaviour(this)
        {
            //Enabled = true
        });
        BehaviourMachine.AddBehaviour(new PlayerDoubleJumpBehaviour(this)
        {
            //Enabled = true
        });

        GameManager.AbilityTypeChanged += AbilityTypeChanged;
        BehaviourMachine.ChangeBehaviour(typeof(PlayerFallingBehaviour));
        ResetOnGrounded();
        InputHandler.JumpButton.OnPress += OnJumpPressed;
        transform.parent = null;

    }

    private void AbilityTypeChanged(GameManager.AbilityType type)
    {
        switch (type)
        {
            case GameManager.AbilityType.DoubleJump:
                AbilityQueue.AddAbility(BehaviourMachine.GetBehaviour<PlayerDoubleJumpBehaviour>());
                break;
            case GameManager.AbilityType.Dash:
                AbilityQueue.AddAbility(BehaviourMachine.GetBehaviour<PlayerDashBehaviour>());
                break;
            case GameManager.AbilityType.Shield:
                AbilityQueue.AddAbility(BehaviourMachine.GetBehaviour<PlayerRockBehaviour>());
                break;
        }
    }

    public void GainAbility(GameManager.AbilityType abilityType)
    {
        GameManager.GainAbility(abilityType);
    }


    void OnDestroy()
    {
        InputHandler.JumpButton.OnPress -= OnJumpPressed;
    }

    public void StartTrail()
    {
        ExternalSpriteTrail.StartTrail(transform.position - Vector3.right * FacingDirection, transform);
    }

    private void OnJumpPressed()
    {
        if (MovementController.Grounded &&
            MovementController.OnOneWayPlatform &&
            InputHandler.MoveAxis.LastValue.y < 0 &&
            Mathf.Abs(InputHandler.MoveAxis.LastValue.y) >= Mathf.Abs(InputHandler.MoveAxis.LastValue.x))
        {
            InputHandler.JumpButton.Pressed = false;
            InputHandler.JumpButton.JustPressed = false;
            InputHandler.JumpButton.TimeLastPressed = TimeContext.Time - PlayerStats.jumpBufferTime * 2;
            MovementController.IgnoreOneWay = true;
        }
    }

    public void SetSpriteDirection(int direction)
    {
        if (direction == 1)
        {
            SpriteRenderer.flipX = false;
        }
        else if (direction == -1)
        {
            SpriteRenderer.flipX = true;
        }
    }

    public bool TryDash()
    {
        return RemainingDashes > 0;
    }

    public void ResetOnGrounded()
    {
        ResetJumps();
        ResetDashes();
    }

    private void ResetJumps()
    {
        Jumps = 0;
    }

    private void ResetDashes()
    {
        RemainingDashes = 1;
    }

    public BehaviourChangeRequest TryUseAbility<T>() where T : BaseBehaviour, IPlayerAbilityBehaviour
    {
        var ability = BehaviourMachine.GetBehaviour<T>();
        if (ability != null && ability.Enabled && !ability.OnCooldown)
        {
            return BehaviourChangeRequest.New<T>();
        }
        return null;
    }

    public void ToggleDashParticles(int value)
    {
        bool enabled = value != 0;
        if (enabled)
        {
            DashParticles.Play();
        }
        else
        {
            DashParticles.Stop();
        }
    }

    void FixedUpdate()
    {
        CheckHazards();
    }

    void CheckHazards()
    {
        List<RaycastHit2D> hits = new();
        ContactFilter2D filter = new()
        {
            layerMask = LayerReference.HazardLayer,
            useLayerMask = true,
        };


        Physics2D.BoxCast(
            transform.position,
            CollisionController.MainCollider.size,
            0,
            MovementController.Velocity.normalized,
            filter,
            hits,
            MovementController.Velocity.magnitude * TimeContext.FixedDeltaTime);

        if (hits.Any(hit => hit))
        {
            foreach (var hit in hits.Where(hit => hit))
            {
                if (hit.collider.GetComponent<BaseHazard>() is BaseHazard hazard)
                {
                    MarkAsDead();
                    switch (hazard.Type)
                    {
                        case BaseHazard.HazardType.Doom:
                            break;
                        case BaseHazard.HazardType.DoubleJump:
                            GainAbility(GameManager.AbilityType.DoubleJump);
                            break;
                        case BaseHazard.HazardType.Shield:
                            GainAbility(GameManager.AbilityType.Shield);
                            break;
                        case BaseHazard.HazardType.Dash:
                            GainAbility(GameManager.AbilityType.Dash);
                            break;
                    }
                }
            }
        }
    }

    public void MarkAsDead()
    {
        IsDead = true;
    }

    public async void Respawn()
    {
        var respawn = GameManager.DieAndGetRespawn();

        if (respawn.respawnType == RespawnType.Hard) // reload scene
        {
            GameManager.DoHardRespawn();
        }
        else
        {

            IsDead = false;
            var startPos = respawn.respawnPosition;
            Debug.Log(respawn.respawnPosition);
            Debug.DrawRay(startPos, Vector2.down * 10, Color.red, 1);
            var groundOffset = PlayerStats.DefaultColliderSize.y / 2;
            var hit = Physics2D.Raycast(startPos, Vector2.down, 10, LayerReference.TerrainLayer);
            if (hit)
            {
                LeanTween.cancelAll();
                MovementController.ForcePosition(hit.point + Vector2.up * groundOffset);
            }
        }

    }

    public void ShowVFXOnPlayer(VFXSpawnData spawnData)
    {
        var offset = spawnData.Offset;
        offset.x *= FacingDirection;
        VFXSpawner.PlayFX(spawnData.VFXClip, transform.position + offset, spawnData.Order, SpriteRenderer.flipX);
    }


    public async void OverrideMovement(params IInputOverride[] inputOverrides)
    {
        // block controls
        InputHandler.BlockInputs(true);
        // execute overrides

        foreach (var inputOverride in inputOverrides)
        {
            switch (inputOverride)
            {
                case ButtonOverride buttonOverride:
                    buttonOverride.Button.OnInputEvent(buttonOverride.Pressed ? UnityEngine.InputSystem.InputActionPhase.Started : UnityEngine.InputSystem.InputActionPhase.Canceled);
                    await Task.Delay(buttonOverride.DurationMilliseconds);
                    buttonOverride.Button.OnInputEvent(UnityEngine.InputSystem.InputActionPhase.Canceled);
                    break;
                case AxisOverride axisOverride:
                    axisOverride.Axis.OnInputEvent(axisOverride.Direction);
                    await Task.Delay(axisOverride.DurationMilliseconds);
                    axisOverride.Axis.OnInputEvent(Vector2.zero);
                    break;
            }
        }

        // unblock controls
        InputHandler.BlockInputs(false);

    }

    public void SetExternalVelocity(Vector2 velocity)
    {
        MovementController.ExternalVelocity = velocity;
    }

    public void Squeeze()
    {
        MarkAsDead();
        GainAbility(GameManager.AbilityType.Shield);
    }
}