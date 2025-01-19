using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillLeeEnemy : MonoBehaviour
{public float moveSpeed = 3f;
    public int maxHealth = 100;
    public float attackRange = 10f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;
    public Transform player;

    private int currentHealth;
    private Rigidbody2D rb;
    private Vector2 movement;
    private float lastAttackTime;

    public GameObject signSlamHitbox; // Reference to the sign hitbox
    public float attackDuration = 0.5f; // Duration of the attack
    public float rotationSpeed = 145f; // Degrees per second

    private int facingDirection = 1; // 1 for right, -1 for left

    private bool isAttacking = false;

    public GameObject lightAttack;

    public GameObject gameManager;

    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        signSlamHitbox.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        animator.SetInteger("currentHealth", currentHealth);
    }

    void Update()
    {
        
        facingDirection = transform.localScale.x > 0 ? -1 : 1;

        // Always move towards the player
        Vector2 direction = (player.position - transform.position).normalized;
        movement = direction;

        // Check if player is in attack range
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            AttackPlayer();
        }
    }

    void FixedUpdate()
    {

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            MoveEnemy();
        }
    }

    void MoveEnemy()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void AttackPlayer()
    {
        if (!isAttacking)
        {
            Debug.Log("Enemy attacks player!");
            if (Random.value < 0.5f)
        {
                StartCoroutine(PerformAttack(signSlamHitbox));
        }
        else
        {
                StartCoroutine(LightAttack());                    
            }
            lastAttackTime = Time.time;
        }
    }

    private IEnumerator PerformAttack(GameObject hitbox)
    {
        isAttacking = true;
       
        yield return new WaitForSeconds(2f);

        isAttacking = false;
    }

    private IEnumerator LightAttack()
    {
        isAttacking = true;
        float spacing = 5f;  // Space between beams
        float beamLifetime = 8f;  // 6 seconds (3 beams * 2 seconds between) + 2 seconds extra
        
        // Determine direction based on facing
        float direction = facingDirection;
        Vector3 spawnPosition = transform.position;
        spawnPosition.x += 2*spacing*direction;

        // Spawn 3 beams with 2 second delays
        for (int i = 0; i < 3; i++)
        {
            GameObject beam = Instantiate(lightAttack, spawnPosition, Quaternion.identity);
            // Destroy this beam after the total lifetime
            Destroy(beam, beamLifetime - (i * 2f));
            
            // Move spawn position for next beam
            spawnPosition.x += spacing * direction;
            
            // Wait 2 seconds before next beam
            if (i < 2)  // Don't wait after the last beam
            {
                yield return new WaitForSeconds(2f);
            }
        }

        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetInteger("currentHealth", currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Add death logic here (e.g., play animation, spawn particles, etc.)
        
        gameManager.GetComponent<LoadNextLevel>().LoadTheNextLevel();
        Destroy(gameObject);
    }

    // Visualize the attack range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

