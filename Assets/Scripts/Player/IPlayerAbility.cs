using UnityEngine;

public interface IPlayerAbility {
    KeyCode AssociatedKey {
        get;
    }
    float CoolDown {
        get;
    }

    //TODO: change to targetID
    void SetTarget(EnemyVM target = null);

    void Execute(EnemyVM enemyVM = null);
}
