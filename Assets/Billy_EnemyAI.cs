using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billy_EnemyAI : MonoBehaviour
{
    [System.Serializable]
    public class AttackData
    {
        public string attackName;
        public float cooldown;
        public float range;
        public float damage;
        public bool isAvailable = true;
        [HideInInspector] public float nextAvailableTime;
    }

    public enum BossState
    {
        Idle,
        Moving,
        Attacking,
        Stunned,
        Dead
    }

    [Header("Boss Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float minDistanceFromPlayer = 3f;
    [SerializeField] private BossState currentState;
    [SerializeField] private float phase2HealthThreshold = 0.5f; // 50% health

    [Header("Attack Settings")]
    [SerializeField] private AttackData[] attacks;
    private int currentAttackIndex = -1;
    [SerializeField] private GameObject beamPrefab;
    [SerializeField] private float beamSpawnDelay = 2f; // Delay between each beam
    [SerializeField] private float beamXOffset = 10f;
    [SerializeField] private float beamDamage = 20f;
    [SerializeField] private float beamLifetime = 1f; // Added for beam lifetime

    // Components
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyHealth health;
    private Transform player;

    // State tracking
    private bool isAttackAnimationPlaying;
    private bool isInPhase2;
    private bool isWaitingForBeams = false;

    public event System.Action OnEnemyDeath;
    public event System.Action<BossState> OnStateChange;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (health != null)
        {
            health.OnEnemyDamaged += HandleDamage;
            health.OnEnemyDeath += HandleDeath;
        }

        currentState = BossState.Idle;
    }

    private void Update()
    {
        if (currentState == BossState.Dead) return;

        UpdateAttackCooldowns();
        CheckPhaseTransition(health.getCurrentHealth() / health.getMaxHealth());

        switch (currentState)
        {
            case BossState.Idle:
                HandleIdleState();
                break;
            case BossState.Moving:
                HandleMovingState();
                break;
            case BossState.Attacking:
                HandleAttackingState();
                break;
            case BossState.Stunned:
                // Handle stunned state
                break;
        }
    }

    private void HandleIdleState()
    {
        // Check for available attacks
        AttackData nextAttack = GetNextAvailableAttack();
        if (nextAttack != null)
        {
            ChangeState(BossState.Attacking);
            return;
        }

        ChangeState(BossState.Moving);
    }

    private void HandleMovingState()
    {
        if(isWaitingForBeams)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // Check if we should attack
        AttackData nextAttack = GetNextAvailableAttack();
        if (nextAttack != null)
        {
            ChangeState(BossState.Attacking);
            return;
        }

        // Move towards player while maintaining minimum distance
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > minDistanceFromPlayer)
        {
            rb.velocity = directionToPlayer * moveSpeed;
            UpdateFacing(directionToPlayer.x);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void HandleAttackingState()
    {
        if (!isAttackAnimationPlaying)
        {
            Debug.Log("Attacking state");
            AttackData attack = GetNextAvailableAttack();
            if (attack != null)
            {
                Debug.Log("Attack found: " + attack.attackName);
                StartAttack(attack);
            }
            else
            {
                ChangeState(BossState.Moving);
            }
        }
    }

    private AttackData GetNextAvailableAttack()
    {
        foreach (var attack in attacks)
        {
            if (attack.isAvailable && Time.time >= attack.nextAvailableTime)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                if (distanceToPlayer <= attack.range)
                {
                    return attack;
                }
            }
        }
        return null;
    }

    private void StartAttack(AttackData attack)
    {
        isAttackAnimationPlaying = true;
        currentAttackIndex = System.Array.IndexOf(attacks, attack);
        animator.SetTrigger(attack.attackName);
        attack.nextAvailableTime = Time.time + attack.cooldown;
    }

    // Called by animation events
    public void OnAttackImpact()
    {
        Debug.Log("OnAttackImpact");
        if (currentAttackIndex >= 0 && currentAttackIndex < attacks.Length)
        {
            Debug.Log("Performing attack: " + attacks[currentAttackIndex].attackName);
            PerformAttack(attacks[currentAttackIndex]);
        }
    }

    // Called by animation events
    public void OnAttackAnimationComplete()
    {
        Debug.Log("OnAttackAnimationComplete");

        isAttackAnimationPlaying = false;

        animator.SetBool("Cough", false);
        animator.SetBool("HolyBeam", false);

        currentAttackIndex = -1;
        ChangeState(BossState.Moving);
    }

    private void PerformAttack(AttackData attack)
    {
        Debug.Log("Performing attack: " + attack.attackName);
        // Implement specific attack logic here
        // This could be different for each attack type
        switch (attack.attackName)
        {
            case "Cough":
                PerformCoughAttack(attack.damage);
                break;
            case "HolyBeam":
                StartCoroutine(SpawnBeamsSequence(attack.damage));
                break;
        }
    }

    private void UpdateAttackCooldowns()
    {
        foreach (var attack in attacks)
        {

            attack.isAvailable = Time.time >= attack.nextAvailableTime;
        }
    }

    private void CheckPhaseTransition(float healthPercent)
    {
        if (!isInPhase2 && healthPercent <= phase2HealthThreshold)
        {
            EnterPhase2();
        }
    }

    private void EnterPhase2()
    {
        isInPhase2 = true;
        animator.SetBool("Phase2", true);

        // Enhance boss abilities, change behavior, etc.
        //moveSpeed *= 1.5f;
        // Maybe unlock new attacks or modify existing ones
    }

    private void UpdateFacing(float directionX)
    {
        if (directionX != 0)
        {
            float yRotation = directionX < 0 ? 180f : 0f;
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }

     public void HandleDamage(float damage)
    {
        if (currentState != BossState.Stunned && currentState != BossState.Dead)
        {
            // Optional: Enter stunned state
        }
    }

    public void HandleDeath()
    {
        Debug.Log("Billy_EnemyAI: HandleDeath");
        AudioController.Instance.FadeOutMusic();
        ChangeState(BossState.Dead);
        OnEnemyDeath?.Invoke();
        FindObjectOfType<LoadNextLevel>().LoadTheNextLevel();
    }

    // Implement specific attack methods
    private void PerformCoughAttack(float damage)
    {
        // Implement melee attack logic
    }

    private void PerformHolyBeamAttack(float damage)
    {
        // Implement ranged attack logic
    }

    private IEnumerator SpawnBeamsSequence(float damage)
    {
        isWaitingForBeams = true;

        // Get the direction based on boss rotation
        float direction = transform.rotation.eulerAngles.y == 0 ? 1 : -1;
        
        // Spawn beams in sequence, each further out
        SpawnBeam(beamXOffset * direction, damage);
        yield return new WaitForSeconds(beamSpawnDelay);
        
        SpawnBeam(beamXOffset * 2 * direction, damage);
        yield return new WaitForSeconds(beamSpawnDelay);
        
        SpawnBeam(beamXOffset * 3 * direction, damage);
        
        // Wait for last beam to finish
        yield return new WaitForSeconds(beamLifetime);

        // Resume and complete animation
        isWaitingForBeams = false;
        animator.speed = 1;
        OnAttackAnimationComplete();
    }

    private void SpawnBeam(float xOffset, float damage)
    {
        if (beamPrefab == null) return;

        // Calculate spawn position based on boss position and offset
        Vector3 spawnPos = transform.position + new Vector3(xOffset, 0, 0);
        
        // Spawn the beam
        GameObject beam = Instantiate(beamPrefab, spawnPos, Quaternion.identity);
        
        // If the beam has a damage component, set its damage
        if (beam.TryGetComponent<HolyBeamAttack>(out var beamAttack))
        {
            beamAttack.SetDamage(damage);
        }
    }

     private void ChangeState(BossState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        OnStateChange?.Invoke(newState);

        if (animator != null)
        {
            animator.SetInteger("State", (int)currentState);
        }
    }
}
