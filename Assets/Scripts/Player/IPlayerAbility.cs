using System;
using UnityEngine;

public interface IPlayerAbility {
    KeyCode AssociatedKey {
        get;
    }
    float CoolDown {
        get;
    }

    //TODO: change to targetID
    void SetTarget(Guid targetID);

    void Execute(IEnemyVM enemyVM);
}
