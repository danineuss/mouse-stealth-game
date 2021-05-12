using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerEvents
{
    event Action<IEnemyVM, bool, Transform> OnSendPlayerLocation;
    event Action<IEnemyVM> OnRemovePlayerLocation;
    event Action<IPlayerAbility> OnAbilityExecuted;
    event Action<IPlayerAbility> OnAbilityLearned;

    void AbilityExecuted(IPlayerAbility ability);
    void AbilityLearned(IPlayerAbility ability);
    void RemovePlayerLocation(IEnemyVM enemyVM);
    void SendPlayerLocation(IEnemyVM enemyVM, bool shouldDisplayText, Transform playerTransform);
}

public class PlayerEvents : IPlayerEvents
{
    public event Action<IEnemyVM, bool, Transform> OnSendPlayerLocation;
    public event Action<IEnemyVM> OnRemovePlayerLocation;
    public event Action<IPlayerAbility> OnAbilityExecuted;
    public event Action<IPlayerAbility> OnAbilityLearned;

    public void SendPlayerLocation(
        IEnemyVM enemyVM, bool shouldDisplayText, Transform playerTransform
    )
    {
        if (OnSendPlayerLocation == null)
            return;

        OnSendPlayerLocation(enemyVM, shouldDisplayText, playerTransform);
    }

    public void RemovePlayerLocation(IEnemyVM enemyVM)
    {
        if (OnRemovePlayerLocation == null)
            return;

        OnRemovePlayerLocation(enemyVM);
    }

    public void AbilityExecuted(IPlayerAbility ability)
    {
        if (OnAbilityExecuted == null)
            return;

        OnAbilityExecuted(ability);
    }

    public void AbilityLearned(IPlayerAbility ability)
    {
        if (OnAbilityLearned == null)
            return;

        OnAbilityLearned(ability);
    }
}
