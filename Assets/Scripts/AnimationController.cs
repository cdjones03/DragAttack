using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AnimationController : MonoBehaviour
{
    public Transform itemSpot;  // Assign this in inspector

    public int currentHealth;
    public int maxHealth = 100;

    public Animator currentAnimator;
    public Animator gownAnimator;
    public Animator suitAnimator;
    private bool isGownForm = true; // Tracks current form
    public float attackCooldown = 0.2f; // Attack cooldown
    private bool canAttack = true;

    // References to the hitboxes for each attack
    public GameObject form1Attack1Hitbox;
    public GameObject form1Attack2Hitbox;
    public GameObject form2Attack1Hitbox;
    public GameObject form2Attack2Hitbox;

    public Image[] healthImages;

    public AudioSource punchSound;
    public AudioSource kickSound;

    private SpriteRenderer spriteRenderer;

    public GameObject gameManager;
    private bool hasItem = false;

    // Start is called before the first frame update
    void Start()
    {
        currentAnimator = gownAnimator;
        // Ensure animator1 is active at start
        DisableAllHitboxes();
        currentHealth = maxHealth;
        healthBarInit();
        Debug.Log("Current Health: " + currentHealth);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleFormSwitch();
        HandleAttacks();

        // Add this for throwing items
        if (Input.GetKeyDown(KeyCode.F) && hasItem)
        {
            ThrowItem();
        }
    }

    private void HandleFormSwitch()
    {
        // Switch forms when the spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentAnimator = currentAnimator == gownAnimator ? suitAnimator : gownAnimator;
            isGownForm = !isGownForm; // Toggle form

            // Debug log to show form change
            Debug.Log(isGownForm ? "Switched to Gown Form" : "Switched to Suit Form");
        }
    }

    private void HandleAttacks()
    {
        // Attack 1 for current form
        if (Input.GetKeyDown(KeyCode.Z) && canAttack)
        {
            punchSound.Play();
            DisableAllHitboxes();

            // Get the hitbox based on current form
            GameObject hitbox = isGownForm ? form1Attack1Hitbox : form2Attack1Hitbox;
            
            // Set hitbox position based on facing direction
            Vector3 hitboxPosition = hitbox.transform.localPosition;
            hitboxPosition.x = Mathf.Abs(hitboxPosition.x) * (spriteRenderer.flipX ? -1 : 1);
            hitbox.transform.localPosition = hitboxPosition;

            StartCoroutine(PerformAttack(hitbox));
        }

        // Attack 2 for current form
        if (Input.GetKeyDown(KeyCode.X) && canAttack)
        {
            kickSound.Play();
            DisableAllHitboxes();

            // Get the hitbox based on current form
            GameObject hitbox = isGownForm ? form1Attack2Hitbox : form2Attack2Hitbox;
            
            // Set hitbox position based on facing direction
            Vector3 hitboxPosition = hitbox.transform.localPosition;
            hitboxPosition.x = Mathf.Abs(hitboxPosition.x) * (spriteRenderer.flipX ? -1 : 1);
            hitbox.transform.localPosition = hitboxPosition;

            StartCoroutine(PerformAttack(hitbox));
        }
    }

    private IEnumerator PerformAttack(GameObject hitbox)
    {
        canAttack = false;

        // Enable the hitbox for the attack
        hitbox.SetActive(true);

        // Wait for the attack duration
        yield return new WaitForSeconds(attackCooldown);

        // Disable the hitbox after the attack
        hitbox.SetActive(false);

        canAttack = true;
    }

    private void DisableAllHitboxes()
    {
        form1Attack1Hitbox.SetActive(false);
        form1Attack2Hitbox.SetActive(false);
        form2Attack1Hitbox.SetActive(false);
        form2Attack2Hitbox.SetActive(false);
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthDisplay();
        if (currentHealth <= 0)
        {
            Die();
        }
        Debug.Log("Current Health: " + currentHealth);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHealthDisplay();
    }

     private void UpdateHealthDisplay()
    {
        // Calculate how many full health images should be shown (1 image = 10 HP)
        int healthImagesToShow = currentHealth / 10;

        // Update each health image
        for (int i = 0; i < healthImages.Length; i++)
        {
            // Enable images if they represent current health, disable if they don't
            healthImages[i].enabled = i < healthImagesToShow;
        }
    }


    private void Die()
    {
    // Start coroutine for delayed level load
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
{
    // Add death effects here (e.g., play animation, spawn particles, etc.)
    
    // Wait for 2 seconds
    yield return new WaitForSeconds(2f);
    
    // Load next level
    if (gameManager != null)
    {
        gameManager.GetComponent<LoadNextLevel>().LoadTheNextLevel();
    }
    else
    {
        Debug.LogError("Game Manager reference is missing!");
    }
    
    // Destroy the enemy
    Destroy(gameObject);
}

    void healthBarInit()
    {
        // Get the first health image
        Image firstHealthImage = healthImages[0];
        
        // Create a list to store all health images
        List<Image> newHealthImages = new List<Image>();
        newHealthImages.Add(firstHealthImage);
        
        // Get the parent canvas/panel
        Transform parentTransform = firstHealthImage.transform.parent;
        
        // Create 9 more health images
        for (int i = 1; i < 10; i++)
        {
            // Instantiate a new image as a copy of the first one
            Image newHealth = Instantiate(firstHealthImage, parentTransform);
            
            // Get the current position
            Vector3 position = newHealth.transform.localPosition;
            
            // Offset the x position by 75 * i
            position.x = firstHealthImage.transform.localPosition.x + (50 * i);
            
            // Set the new position
            newHealth.transform.localPosition = position;
            
            // Add to our list
            newHealthImages.Add(newHealth);
        }
        
        // Update the healthImages array with our new list
        healthImages = newHealthImages.ToArray();
    }

    void ThrowItem()
    {
        if (hasItem)
        {
            ItemPickup item = itemSpot.GetComponentInChildren<ItemPickup>();
            if (item != null)
            {
                Vector2 throwDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;
                item.ThrowItem(throwDirection, transform);  // Pass the player's transform
                hasItem = false;
            }
        }
    }

    public bool IsGownForm() { return isGownForm; }

    // Helper methods for item management
    public bool HasItem() { return hasItem; }
    public void SetHasItem(bool value) { hasItem = value; }
    public Transform GetItemSpot() { return itemSpot; }
}
