using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityDistract : MonoBehaviour, IPlayerAbility {
    public KeyCode AssociatedKey {
        get => KeyCode.F;
    }
    public float CoolDown {
        get => 10f;
    }

    public bool Execute(EnemyVM enemyVM = null) {
        if (enemyVM == null)
            return false;

        return enemyVM.GetDistracted();
    }
}
