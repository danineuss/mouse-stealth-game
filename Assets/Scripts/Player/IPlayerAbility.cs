using System;
using UnityEngine;

public interface IPlayerAbility {
    KeyCode AssociatedKey { get; }
    float CoolDown { get; }

    void SetTarget(Guid targetID);

    void Execute(IEnemyVM enemyVM);
}
