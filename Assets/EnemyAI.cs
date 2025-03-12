using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
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
    private Rigidbody2D rb;
    private EnemyHealth health;
    private Animator animator;

    // State
    [SerializeField] private EnemyState currentState = EnemyState.Idle;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float detectionRange = 10f;

    // References
    private Transform player;
    private bool isInBattle = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //Debug.Log("Player found: " + player);
        // Subscribe to health events
        if (health != null)
        {
            health.OnEnemyDamaged += HandleDamage;
            health.OnEnemyDeath += HandleDeath;
        }
    }

    public void SetBattleState(bool inBattle)
    {
        isInBattle = inBattle;
        if (inBattle)
        {
            ChangeState(EnemyState.Moving);
        }
    }

    // Update is called once per frame
    void Update()
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
                // Handle stunned state
                break;
        }
    }

    private void HandleIdleState()
    {
        Debug.Log("Idle State");
        if (IsPlayerInRange(detectionRange))
        {
            Debug.Log("Player in range");
            ChangeState(EnemyState.Moving);
        }
    }

    private void HandleMovingState()
    {
        if (IsPlayerInRange(attackRange))
        {
            ChangeState(EnemyState.Attacking);
            return;
        }

        // Move towards player
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        // Update facing direction
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(direction.x), 
                transform.localScale.y, 
                transform.localScale.z
            );
        }
    }

    private void HandleAttackingState()
    {
        rb.velocity = Vector2.zero;
        if (!IsPlayerInRange(attackRange))
        {
            ChangeState(EnemyState.Moving);
        }


        // Attack logic here
    }

    private bool IsPlayerInRange(float range)
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= range;
    }

    private void HandleDamage(float damage)
    {
        // Handle damage effects, animations, etc.
        if (currentState != EnemyState.Stunned && currentState != EnemyState.Dead)
        {
            // Optionally enter stunned state
            // ChangeState(EnemyState.Stunned);
        }
    }

    private void HandleDeath()
    {
        ChangeState(EnemyState.Dead);
        OnEnemyDeath?.Invoke();
        // Handle death effects, animations, etc.
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        OnStateChange?.Invoke(newState);

        // Update animator
        if (animator != null)
        {
            animator.SetInteger("State", (int)currentState);
        }
    }

    public void ApplyKnockback(Vector2 force)
    {
        if (currentState == EnemyState.Dead) return;
        
        rb.velocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
        ChangeState(EnemyState.Stunned);
        
        // Optional: Add a coroutine to recover from stun
        StartCoroutine(RecoverFromKnockback());
    }

    private System.Collections.IEnumerator RecoverFromKnockback()
    {
        yield return new WaitForSeconds(0.5f); // Adjust stun duration
        if (currentState == EnemyState.Stunned)
        {
            ChangeState(EnemyState.Moving);
        }
    }
}
