using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechAttack : MonoBehaviour
{
[Header("Projectile Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private bool followTarget = true;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    
    private Transform target;
    private bool hasHit = false;

    private void Start()
    {
        // Get rigidbody if not set
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        
        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (hasHit) return;

        if (followTarget && target != null)
        {
            // Calculate direction to target
            Vector2 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * speed;

            // Optional: Rotate projectile to face direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void Initialize(Transform target)
    {
        this.target = target;
        
        if (!followTarget)
        {
            // If not following target, just set initial direction
            Vector2 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        Debug.Log("SpeechAttack hit: " + other.name);

        // Check if we hit something damageable
        if (other.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            playerHealth.Damage(damage);
            hasHit = true;
            
            // Optional: Play hit effect
            // Instantiate(hitEffect, transform.position, Quaternion.identity);
            
            Destroy(gameObject);
        }
    }
}