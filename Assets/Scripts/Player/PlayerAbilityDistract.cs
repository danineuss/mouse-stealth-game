using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityDistract : IPlayerAbility 
{
    public KeyCode AssociatedKey => KeyCode.F;
    public float CoolDown => 10f;

    private EnemyVM targetEnemy;

    public void SetTarget(EnemyVM target)
    {
        this.targetEnemy = target;
    }

    public void Execute(EnemyVM enemyVM = null) 
    {
        if (enemyVM == null || enemyVM != targetEnemy)
            return;

        enemyVM.GetDistracted();
    }
}
