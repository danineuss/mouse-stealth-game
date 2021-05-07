using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityDistract : IPlayerAbility {
    public KeyCode AssociatedKey => KeyCode.F;
    public float CoolDown => 10f;

    public bool Execute(EnemyVM enemyVM = null) {
        if (enemyVM == null)
            return false;

        return enemyVM.GetDistracted();
    }
}
