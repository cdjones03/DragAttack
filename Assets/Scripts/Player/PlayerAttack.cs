using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Suit Form")]
    [SerializeField] private Transform suitPunchTransform;
    [SerializeField] private Transform suitKickTransform;
    [SerializeField] private float suitPunchRange = 1.5f;
    [SerializeField] private float suitKickRange = 2f;

    [Header("Gown Form")]
    [SerializeField] private Transform gownPunchTransform;
    [SerializeField] private Transform gownKickTransform;
    [SerializeField] private float gownPunchRange = 1.8f;
    [SerializeField] private float gownKickRange = 2.2f;

    [Header("General Settings")]
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private GameObject punchHitbox;
    [SerializeField] private GameObject kickHitbox;

    private RaycastHit2D[] hits;
    private Animator animator;
    private bool isInGownForm;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(UserInput.instance.controls.Attack.Punch.WasPressedThisFrame())
        {
            Punch();
            animator.SetTrigger("Punch");
        }
        else if(UserInput.instance.controls.Attack.Kick.WasPressedThisFrame())
        {
            Kick();
            animator.SetTrigger("Kick");
        }
    }

    private void Punch()
    {
        //punchHitbox.SetActive(true);
        hits = Physics2D.CircleCastAll(suitPunchTransform.position, suitPunchRange, suitKickTransform.right, 0f, attackableLayer);
        for(int i = 0; i < hits.Length; i++)
        {
            IDamageable iDamageable = hits[i].collider.GetComponent<IDamageable>();
            if(iDamageable != null)
            {
                iDamageable.Damage(1);
            }   
        }
    }

    private void Kick()
    {
        //kickHitbox.SetActive(true);
    }

    private void OnDrawGizmosSelected()
    {
       Gizmos.DrawWireSphere(suitPunchTransform.position, suitPunchRange);
    }
    
    // Update this method for single animator
    public void UpdateForm(bool inGownForm)
    {
        isInGownForm = inGownForm;
        animator.SetBool("IsGownForm", isInGownForm);
    }
}
