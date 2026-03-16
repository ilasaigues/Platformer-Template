using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(CollisionController))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(BehaviourMachine))]
[RequireComponent(typeof(PlayerAnimator))]
public class PlayerController : MonoBehaviour
{
    public PlayerStats PlayerStats;
    public AbilityStats AbilityStats;
    public CollisionController CollisionController;
    public MovementController MovementController;
    public PlayerAnimator PlayerAnimator;
    public InputHandler InputHandler;

    public SpriteTrail ExternalSpriteTrail;

    public BehaviourMachine BehaviourMachine;

    public ParticleSystem DashParticles;

    public Vector2 LastDirectionInput => InputHandler.MoveAxis.LastValue;
    public Vector2 LastHorizontalDirection { get; private set; }

    public int RemainingDashes = 1;

    public int Jumps = 0;

    public bool IsDead;

    public SpriteRenderer SpriteRenderer;

    public int FacingDirection => SpriteRenderer.flipX ? -1 : 1;


    public PlayerAbilityQueue PlayerAbilityQueue = new();


    RespawnTrigger _currentRespawnTrigger;

    public int RemainingLives { get; private set; }

    void Start()
    {
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
        PlayerAbilityQueue.MaxAbilityStack = 1;


        BehaviourMachine.ChangeBehaviour(typeof(PlayerFallingBehaviour));
        ResetOnGrounded();
        InputHandler.JumpButton.OnPress += OnJumpPressed;
    }

    public void GainAbility<T>() where T : BasePlayerBehaviour, IPlayerAbilityBehaviour
    {
        PlayerAbilityQueue.AddAbility(BehaviourMachine.GetBehaviour<T>());
    }


    void OnDestroy()
    {
        InputHandler.JumpButton.OnPress -= OnJumpPressed;
    }

    public void StartTrail()
    {
        ExternalSpriteTrail?.StartTrail(transform.position, transform);
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
            InputHandler.JumpButton.TimeLastPressed = Time.time - PlayerStats.jumpBufferTime * 2;
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

    public void SetRespawn(RespawnTrigger respawn)
    {
        _currentRespawnTrigger = respawn;
    }

    public void ToggleDashParticles(bool enabled)
    {
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
            MovementController.Velocity.magnitude * Time.fixedDeltaTime);

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
                            GainAbility<PlayerDoubleJumpBehaviour>();
                            break;
                        case BaseHazard.HazardType.Shield:
                            GainAbility<PlayerRockBehaviour>();
                            break;
                        case BaseHazard.HazardType.Dash:
                            GainAbility<PlayerDashBehaviour>();
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

    public void Respawn()
    {
        IsDead = false;
        if (_currentRespawnTrigger != null)
        {
            var startPos = _currentRespawnTrigger.RespawnPosition;
            var groundOffset = PlayerStats.DefaultColliderSize.y / 2;
            var hit = Physics2D.Raycast(startPos, Vector2.down, 10, LayerReference.TerrainLayer);
            if (hit)
            {
                MovementController.ForcePosition(hit.point + Vector2.up * groundOffset);
            }
        }
        else
        {
            Debug.LogError("NO RESPAWN SET LMAO");
        }
    }

    public void ShowVFXOnPlayer(VFXSpawnData spawnData)
    {
        var offset = spawnData.Offset;
        offset.x *= FacingDirection;
        VFXSpawner.Instance.PlayFX(spawnData.VFXClip, transform.position + offset, SpriteRenderer.flipX);
    }

}