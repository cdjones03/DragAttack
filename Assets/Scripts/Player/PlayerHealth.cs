using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float currentHealth;

    public event System.Action OnPlayerDeath;
    public event System.Action<float> OnPlayerDamaged;
    public event System.Action<float> OnPlayerHealed;
    private HPBar playerHPBar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        playerHPBar = FindObjectOfType<HPBar>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            Damage(10f);
        }
    }

    public void Damage(float damageAmount)
    {
        if(currentHealth <= 0)
        {
            return;
        }

        currentHealth -= damageAmount;
        OnPlayerDamaged?.Invoke(currentHealth);

        Debug.Log("Player took damage: " + damageAmount);
        Debug.Log("Player current health: " + currentHealth);

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        OnPlayerHealed?.Invoke(currentHealth);
    }

    public void Die()
    {
        Debug.Log("Player died");
        AudioController.Instance.FadeOutMusic();
        UserInput.instance.DisableInput();
        OnPlayerDeath?.Invoke();
        FindObjectOfType<LoadNextLevel>().LoadTheNextLevel();
        //Destroy(gameObject);
    }

    public float getMaxHealth()
    {
        return maxHealth;
    }

    public float getCurrentHealth()  
    {
        return currentHealth;
    }
    
}
