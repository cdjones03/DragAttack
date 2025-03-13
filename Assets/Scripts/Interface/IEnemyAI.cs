using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyAI
{
    void SetBattleState(bool inBattle);
    void HandleDamage(float damage);
    void ApplyKnockback(Vector2 force);
}
