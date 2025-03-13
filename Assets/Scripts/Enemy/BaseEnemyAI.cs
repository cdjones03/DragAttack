using UnityEngine;

public abstract class BaseEnemyAI : MonoBehaviour, IEnemyAI
{
    // States
    public enum EnemyState
    {
        Idle,
        Moving,
        Attacking,
        Stunned,
        Dead
    }

    // Events
    public event System.Action OnEnemyDeath;
    public event System.Action<EnemyState> OnStateChange;

    // Components
    protected Rigidbody2D rb;
    protected EnemyHealth health;
    protected Animator animator;

    // State
    [SerializeField] protected EnemyState currentState = EnemyState.Idle;
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float detectionRange = 10f;

    // References
    protected Transform player;
    protected bool isInBattle = true;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (health != null)
        {
            health.OnEnemyDamaged += HandleDamage;
            health.OnEnemyDeath += HandleDeath;
        }
    }

    public virtual void SetBattleState(bool inBattle)
    {
        isInBattle = inBattle;
        if (inBattle)
        {
            ChangeState(EnemyState.Moving);
        }
    }

    protected virtual void Update()
    {
        if (!isInBattle || currentState == EnemyState.Dead) return;

        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                break;
            case EnemyState.Moving:
                HandleMovingState();
                break;
            case EnemyState.Attacking:
                HandleAttackingState();
                break;
            case EnemyState.Stunned:
                HandleStunnedState();
                break;
        }
    }

    protected virtual void HandleIdleState()
    {
        if (IsPlayerInRange(detectionRange))
        {
            ChangeState(EnemyState.Moving);
        }
    }

    protected virtual void HandleMovingState()
    {
        if (IsPlayerInRange(attackRange))
        {
            ChangeState(EnemyState.Attacking);
            return;
        }

        MoveTowardsPlayer();
    }

    protected virtual void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        // Update facing direction using Y rotation
        if (direction.x != 0)
        {
            // If moving left, rotate 180 degrees around Y axis
            float yRotation = direction.x < 0 ? 180f : 0f;
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }

    protected abstract void HandleAttackingState();
    protected virtual void HandleStunnedState() { }

    protected bool IsPlayerInRange(float range)
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= range;
    }

    public virtual void HandleDamage(float damage)
    {
        if (currentState != EnemyState.Stunned && currentState != EnemyState.Dead)
        {
            // Optional: Enter stunned state
        }
    }

    protected virtual void HandleDeath()
    {
        ChangeState(EnemyState.Dead);
        OnEnemyDeath?.Invoke();
    }

    protected virtual void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        OnStateChange?.Invoke(newState);

        if (animator != null)
        {
            animator.SetInteger("State", (int)currentState);
        }
    }

    public virtual void ApplyKnockback(Vector2 force)
    {
        if (currentState == EnemyState.Dead) return;
        
        rb.velocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
        ChangeState(EnemyState.Stunned);
        StartCoroutine(RecoverFromKnockback());
    }

    protected virtual System.Collections.IEnumerator RecoverFromKnockback()
    {
        yield return new WaitForSeconds(0.5f);
        if (currentState == EnemyState.Stunned)
        {
            ChangeState(EnemyState.Moving);
        }
    }
} 