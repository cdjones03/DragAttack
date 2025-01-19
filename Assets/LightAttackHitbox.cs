using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAttackHitbox : MonoBehaviour
{
    private int damage = 20; // Damage dealt by the light
    public GameObject player;
    public float speed = 5f;
    public float destroyDelay = 0.5f;

    private Transform playerTransform;
    private bool hasCollided = false;

    public float attackDuration = 3f;
    
    private void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object is an enemy
        if (other.CompareTag("Player") && !hasCollided)
        {
            // Call the enemy's TakeDamage function (if it has one)
            AnimationController player = other.GetComponent<AnimationController>();
            if (player != null)
            {
                hasCollided = true;
                player.TakeDamage(damage);
            }
        }
    }

    private void Update()
    {
        
    }

     private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
