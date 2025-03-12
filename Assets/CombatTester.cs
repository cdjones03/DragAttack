using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTester : MonoBehaviour
{
    [SerializeField] public bool canAttack = true;
    
    [SerializeField] private Collider2D inLineCollider;
    
    [SerializeField] private LayerMask enemyLayer;
    private ContactFilter2D contactFilter2D;
    public List<Collider2D> cols = new List<Collider2D>();

    private bool punchInput;
    private bool kickInput;
    private bool switchFormInput;
    private bool throwInput;

    private bool isGownForm;

    private Animator animator;

    [SerializeField] private float formSwitchCooldown = 1f; // Cooldown duration in seconds
    private float nextFormSwitchTime = 0f; // Time when next switch is allowed

    [SerializeField] private float alignmentTolerance = 0.5f; // How close the player and enemy need to be on the X-axis to be considered aligned.

    [SerializeField] private float attackCooldown = 0.5f;
    private float nextAttackTime = 0f;

    private HashSet<int> attacksHit = new HashSet<int>();  // Tracks individual attacks
    private HashSet<GameObject> enemiesHit = new HashSet<GameObject>();  // Tracks enemies hit by the attack
    private int currentAttackID;

    [SerializeField] private float punchDamage = 10f;
    [SerializeField] private float kickDamage = 10f;

    private void Awake()
    {
        contactFilter2D.SetLayerMask(enemyLayer);
    }

    private void Start()
    {
        isGownForm = true;
        animator = GetComponentInChildren<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        punchInput = UserInput.instance.punchInput;
        kickInput = UserInput.instance.kickInput;
        switchFormInput = UserInput.instance.switchFormInput;
        throwInput = UserInput.instance.throwInput;

        if (switchFormInput && Time.time >= nextFormSwitchTime)
        {
            isGownForm = !isGownForm;
            animator.SetBool("isGownForm", isGownForm);
            nextFormSwitchTime = Time.time + formSwitchCooldown;
        }

        if (Time.time >= nextAttackTime)
        {
                if (punchInput)
                {
                    PerformPunch();
                    nextAttackTime = Time.time + attackCooldown;
                }


                if (kickInput)
                {
                    PerformKick();
                    nextAttackTime = Time.time + attackCooldown;
                }
        }
    }

    private void PerformPunch()
    {
        // Generate unique ID for this attack instance
        currentAttackID = Random.Range(0, int.MaxValue);
        enemiesHit.Clear();  // Reset the hit tracking for new attack
        
        animator.SetBool("isPunching", true);
        Debug.Log("Starting new punch attack with ID: " + currentAttackID);

        if (IsAlignedWithEnemy())
        {
            Debug.Log("Enemy aligned, checking collisions");
            inLineCollider.OverlapCollider(contactFilter2D, cols);
            if (cols.Count > 0)
            {
                Debug.Log("Found " + cols.Count + " colliders");
                foreach (var col in cols)
                {
                    if (col.CompareTag("Player"))
                    {
                        Debug.Log("Skipping Player collision");
                        continue;
                    }

                    // Create unique ID for this specific hit
                    int hitID = col.gameObject.GetInstanceID() + currentAttackID;
                    
                    Debug.Log("Checking hit ID: " + hitID + " for object: " + col.name);
                    
                    // Skip if we've already processed this hit
                    if (attacksHit.Contains(hitID))
                    {
                        Debug.Log("Already hit this target in this attack");
                        continue;
                    }
                    if (enemiesHit.Contains(col.gameObject))
                    {
                        Debug.Log("Already hit this enemy this attack");
                        continue;
                    }

                    // Mark this hit as processed
                    attacksHit.Add(hitID);
                    enemiesHit.Add(col.gameObject);

                    Debug.Log("Hit registered on: " + col.transform.name);
                    if (col.TryGetComponent(out SpriteRenderer sr))
                    {
                        sr.color = Color.red;
                    }

                    var damageable = col.GetComponentInParent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.Damage(punchDamage);
                        Debug.Log("Damage dealt to: " + col.name);
                    }
                }
            }
            else
            {
                Debug.Log("No colliders found in range");
            }
        }
        else
        {
            Debug.Log("Not aligned with enemy");
        }
    }

    private void PerformKick()
    {
        Debug.Log("Performing kick");
        // Generate unique ID for this attack instance
        currentAttackID = Random.Range(0, int.MaxValue);
        enemiesHit.Clear();  // Reset the hit tracking for new attack
        
        animator.SetBool("isKicking", true);
        Debug.Log("Starting new kick attack with ID: " + currentAttackID);

        if (IsAlignedWithEnemy())
        {
            Debug.Log("Enemy aligned, checking collisions");
            inLineCollider.OverlapCollider(contactFilter2D, cols);
            if (cols.Count > 0)
            {
                Debug.Log("Found " + cols.Count + " colliders");
                foreach (var col in cols)
                {
                    if (col.CompareTag("Player"))
                    {
                        Debug.Log("Skipping Player collision");
                        continue;
                    }

                    // Create unique ID for this specific hit
                    int hitID = col.gameObject.GetInstanceID() + currentAttackID;
                    
                    Debug.Log("Checking hit ID: " + hitID + " for object: " + col.name);
                    
                    // Skip if we've already processed this hit
                    if (attacksHit.Contains(hitID))
                    {
                        Debug.Log("Already hit this target in this attack");
                        continue;
                    }
                    if (enemiesHit.Contains(col.gameObject))
                    {
                        Debug.Log("Already hit this enemy this attack");
                        continue;
                    }

                    // Mark this hit as processed
                    attacksHit.Add(hitID);
                    enemiesHit.Add(col.gameObject);

                    Debug.Log("Hit registered on: " + col.transform.name);
                    if (col.TryGetComponent(out SpriteRenderer sr))
                    {
                        sr.color = Color.red;
                    }

                    var damageable = col.GetComponentInParent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.Damage(kickDamage);
                        Debug.Log("Damage dealt to: " + col.name);
                    }
                }
            }
            else
            {
                Debug.Log("No colliders found in range");
            }
        }
        else
        {
            Debug.Log("Not aligned with enemy");
        }
    }

    // Function to check if the player and enemy are aligned on the y-axis
    private bool IsAlignedWithEnemy()
    {
        if (inLineCollider != null)
        {
            Collider2D[] hitColliders = new Collider2D[10];
            int colliderCount = inLineCollider.OverlapCollider(contactFilter2D, hitColliders);

            for (int i = 0; i < colliderCount; i++) // Iterate through only the valid hit colliders
            {
                if (hitColliders[i] != null && hitColliders[i].CompareTag("Enemy"))
                {
                    // Check if the player and enemy are aligned on the X-axis within a tolerance range
                    float yDistance = Mathf.Abs(transform.position.y - hitColliders[i].transform.position.y);
                    //Debug.Log("yDistance: " + yDistance);
                    if (yDistance <= alignmentTolerance)
                    {
                        return true; // They are aligned within the tolerance
                    }
                }
            }
        }
        return false; // Not aligned
    }

}

