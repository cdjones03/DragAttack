using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public float maxHP;
    public float currentHP;
    public Image[] heartIcons;
    public PlayerHealth playerHealth;

    private void Start()
    {
        if (playerHealth != null)
        {
            // Subscribe to player health events
            playerHealth.OnPlayerDamaged += UpdateHPBar;
            playerHealth.OnPlayerHealed += UpdateHPBar;
            
            // Initialize health display
            maxHP = playerHealth.getMaxHealth();
            currentHP = playerHealth.getCurrentHealth();
            UpdateHPBar(currentHP);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe when destroyed to prevent memory leaks
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDamaged -= UpdateHPBar;
            playerHealth.OnPlayerHealed -= UpdateHPBar;
        }
    }

    private void UpdateHPBar(float newHP)
    {
        currentHP = (int)newHP;
        
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (i * 10 < currentHP)
            {
                heartIcons[i].enabled = true;
            }
            else
            {
                heartIcons[i].enabled = false;
            }
        }
    }
}
