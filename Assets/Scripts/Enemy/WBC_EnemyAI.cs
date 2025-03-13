using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WBC_EnemyAI : BaseEnemyAI
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackCooldown = 3f;
    private float nextAttackTime;

    protected override void HandleAttackingState()
    {
        rb.velocity = Vector2.zero;
        
        if (Time.time >= nextAttackTime)
        {
            ShootProjectile();
            nextAttackTime = Time.time + attackCooldown;
        }

        if (!IsPlayerInRange(attackRange))
        {
            ChangeState(EnemyState.Moving);
        }
    }

    private void ShootProjectile()
    {
        if (player == null) return;

        GameObject projectileObj = Instantiate(
            projectilePrefab, 
            transform.position, 
            Quaternion.identity
        );

        if (projectileObj.TryGetComponent<SpeechAttack>(out var projectile))
        {
            projectile.Initialize(player);
        }
    }
}
