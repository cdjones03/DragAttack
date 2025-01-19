using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyHitboxDamage : MonoBehaviour
{
    private int damage = 5; // Damage dealt by the punch
    public GameObject player;
     public float speed = 5f;
    public float destroyDelay = 0.5f;

    private Transform playerTransform;
    private bool hasCollided = false;
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found. Make sure the player object has the 'Player' tag.");
        }
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
                
                player.TakeDamage(damage);
                StartCoroutine(DestroyAfterDelay());
            }
        }
    }

     private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
