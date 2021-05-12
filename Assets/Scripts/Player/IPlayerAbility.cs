using UnityEngine;

public interface IPlayerAbility {
    KeyCode AssociatedKey {
        get;
    }
    float CoolDown {
        get;
    }

    //TODO: change to targetID
    void SetTarget(IEnemyVM target = null);

    void Execute(IEnemyVM enemyVM = null);
}
