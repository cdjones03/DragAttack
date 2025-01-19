using UnityEngine;

public class BitchSlapHitboxDamage : MonoBehaviour
{
    public int damage = 10; // Damage dealt by the punch

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object is an enemy
        if (other.CompareTag("Enemy"))
        {
            // Call the enemy's TakeDamage function (if it has one)
            Enemy enemy = other.GetComponent<Enemy>();
            BillLeeEnemy bossEnemy = other.GetComponent<BillLeeEnemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            if (bossEnemy != null)
            {
                Debug.Log("Hit boss enemy for " + damage + " damage!");
                bossEnemy.TakeDamage(damage);
            }
        }
    }
}
