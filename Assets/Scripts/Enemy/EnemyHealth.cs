using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 30;
    private float currentHealth;

    public event System.Action OnEnemyDeath;
    public event System.Action<float> OnEnemyDamaged;
    public event System.Action<float> OnEnemyHealed;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(float damageAmount)
    {
        if(currentHealth <= 0)
        {
            return;
        }

        currentHealth -= damageAmount;
        OnEnemyDamaged?.Invoke(damageAmount);

        Debug.Log("Enemy took damage: " + damageAmount);
        Debug.Log("Enemy current health: " + currentHealth);

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Enemy died");
        OnEnemyDeath?.Invoke();
        Destroy(gameObject);
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        OnEnemyHealed?.Invoke(healAmount);
    }

    public float getCurrentHealth()
    {
        return currentHealth;
    }

    public float getMaxHealth()
    {
        return maxHealth;
    }
        
}
