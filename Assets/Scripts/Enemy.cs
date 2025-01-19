using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 3f;
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

    public GameObject speechAttack;

    public GameObject healDrink;
    public GameObject throwDrink;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        signSlamHitbox.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player").transform;
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
        MoveEnemy();
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
                StartCoroutine(SpeechAttack());
            }
            lastAttackTime = Time.time;
        }
    }

    private IEnumerator PerformAttack(GameObject hitbox)
    {
        isAttacking = true;
        signSlamHitbox.SetActive(true);

        // Calculate target rotation based on facing direction
        float totalRotation = 0f;
        float targetRotation = 90f * facingDirection; // Adjust rotation direction

        // Rotate the hitbox over the attack duration
        while (Mathf.Abs(totalRotation) < 90f)
        {
            float rotationStep = rotationSpeed * Time.deltaTime;
            rotationStep *= facingDirection; // Flip the rotation direction

            signSlamHitbox.transform.Rotate(0, 0, -rotationStep); // Rotate the hitbox
            totalRotation += Mathf.Abs(rotationStep);

            yield return null; // Wait for the next frame
        }

        // Wait for the remaining attack duration
        yield return new WaitForSeconds(attackDuration - (90f / rotationSpeed));

        // Reset the rotation
        signSlamHitbox.transform.rotation = Quaternion.identity;
        signSlamHitbox.SetActive(false);

        isAttacking = false;
    }

    private IEnumerator SpeechAttack()
    {
        isAttacking = true;
        Instantiate(speechAttack, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Random number between 0 and 2 (inclusive)
        int randomDrop = Random.Range(0, 3);

        // Spawn based on random number
        switch (randomDrop)
        {
            case 0:
                // Spawn health drink
                if (healDrink != null)
                {
                    Instantiate(healDrink, transform.position, Quaternion.identity);
                }
                break;
            case 1:
                // Spawn throw drink
                if (throwDrink != null)
                {
                    Instantiate(throwDrink, transform.position, Quaternion.identity);
                }
                break;
            case 2:
                // Spawn nothing
                break;
        }

        // Destroy the enemy
        Destroy(gameObject);
    }

    // Visualize the attack range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
