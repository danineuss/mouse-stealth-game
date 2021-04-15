using UnityEngine;

public interface IPlayerAbility
{
    KeyCode AssociatedKey {
        get;
    }
    float CoolDown {
        get;
    }

    bool Execute(EnemyVM enemyVM = null);
}
