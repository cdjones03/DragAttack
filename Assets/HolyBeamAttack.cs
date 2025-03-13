using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyBeamAttack : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float warningTime = 1f;    // How long to show warning beam
    [SerializeField] private float damageTime = 0.5f;   // How long to show damage beam
    
    [Header("References")]
    [SerializeField] private Sprite warningBeamSprite;  // Thin warning beam
    [SerializeField] private Sprite damageBeamSprite;   // Full damage beam
    
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D beamCollider;
    private float damage;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        beamCollider = GetComponent<BoxCollider2D>();
        beamCollider.enabled = false;
        
        // Start with warning beam
        spriteRenderer.sprite = warningBeamSprite;
        
        StartCoroutine(BeamSequence());
    }

    private IEnumerator BeamSequence()
    {
        // Warning phase - thin beam
        yield return new WaitForSeconds(warningTime);
        
        // Damage phase - full beam
        spriteRenderer.sprite = damageBeamSprite;
        beamCollider.enabled = true;
        
        yield return new WaitForSeconds(damageTime);
        
        // Cleanup
        Destroy(gameObject);
    }

    public void SetDamage(float amount)
    {
        damage = amount;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.Damage(damage);
            }
        }
    }
}


//spawn
//after 2 seconds move to 2nd sprite
//after 3 seconds despawn