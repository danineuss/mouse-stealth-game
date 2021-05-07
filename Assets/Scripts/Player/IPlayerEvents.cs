using System;
using UnityEngine;

public interface IPlayerEvents
{
    event Action<EnemyVM, bool, Transform> OnSendPlayerLocation;
    event Action<EnemyVM> OnRemovePlayerLocation;
    event Action<IPlayerAbility> OnAbilityExecuted;
    event Action<IPlayerAbility> OnAbilityLearned;

    void AbilityExecuted(IPlayerAbility ability);
    void AbilityLearned(IPlayerAbility ability);
    void RemovePlayerLocation(EnemyVM enemyVM);
    void SendPlayerLocation(EnemyVM enemyVM, bool shouldDisplayText, Transform playerTransform);
}
