using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTester : MonoBehaviour
{
    [SerializeField] private bool canAttack = true;
    
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

        if (punchInput)
        {
            animator.SetBool("isPunching", true);

            inLineCollider.OverlapCollider(contactFilter2D, cols);
            if (cols.Count > 0)
            {
                foreach (var col in cols)
                {
                    print(col.transform.name);
                    if (col.TryGetComponent(out SpriteRenderer sr))
                    {
                        //sr.color = Color.red;
                    }
                }
            }
        }

        if (kickInput)
        {
            animator.SetBool("isKicking", true);
        }
    }

}
