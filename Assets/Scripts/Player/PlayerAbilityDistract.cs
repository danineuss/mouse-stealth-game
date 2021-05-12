using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityDistract : IPlayerAbility 
{
    public KeyCode AssociatedKey => KeyCode.F;
    public float CoolDown => 10f;

    private IEnemyVM targetEnemy;

    public void SetTarget(IEnemyVM target)
    {
        this.targetEnemy = target;
    }

    public void Execute(IEnemyVM enemyVM = null) 
    {
        if (enemyVM == null || enemyVM != targetEnemy)
            return;

        enemyVM.GetDistracted();
    }
}
