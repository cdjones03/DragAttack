using UnityEngine;
using System.Collections;
public class BitchSlapHitbox : MonoBehaviour
{
    public GameObject punchHitbox; // Reference to the hitbox
    public float attackDuration = 0.2f; // Duration of the punch hitbox

    private bool isAttacking = false;
    

    void Start()
    {
        // Ensure the hitbox is initially disabled
        punchHitbox.SetActive(false);
        punchHitbox.GetComponent<SpriteRenderer>().enabled = false;
    }

    void Update()
    {
        // Check for punch input (e.g., spacebar)
        if (Input.GetKeyDown(KeyCode.Z) && !isAttacking)
        {
            StartCoroutine(PerformPunch());
        }
    }

    private IEnumerator PerformPunch()
    {
        isAttacking = true;

        // Enable the punch hitbox
        punchHitbox.SetActive(true);
        punchHitbox.GetComponent<SpriteRenderer>().enabled = true;


        // Wait for the duration of the attack
        yield return new WaitForSeconds(attackDuration);

        // Disable the punch hitbox
        punchHitbox.SetActive(false);
        punchHitbox.GetComponent<SpriteRenderer>().enabled = false;


        isAttacking = false;
    }
}
