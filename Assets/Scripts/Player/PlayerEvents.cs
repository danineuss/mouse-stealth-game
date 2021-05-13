using System;
using UnityEngine;

public interface IPlayerEvents
{
    event Action<IEnemyVM, bool, Transform> OnSendPlayerLocation;
    event Action<IEnemyVM> OnRemovePlayerLocation;
    event Action<IPlayerAbility> OnAbilityExecuted;
    event Action<IPlayerAbility> OnAbilityLearned;
    event Action OnPauseButtonPressed;

    void AbilityExecuted(IPlayerAbility ability);
    void AbilityLearned(IPlayerAbility ability);
    void RemovePlayerLocation(IEnemyVM enemyVM);
    void SendPlayerLocation(IEnemyVM enemyVM, bool shouldDisplayText, Transform playerTransform);
    void PressPauseButton();
}

public class PlayerEvents : IPlayerEvents
{
    public event Action<IEnemyVM, bool, Transform> OnSendPlayerLocation;
    public event Action<IEnemyVM> OnRemovePlayerLocation;
    public event Action<IPlayerAbility> OnAbilityExecuted;
    public event Action<IPlayerAbility> OnAbilityLearned;
    public event Action OnPauseButtonPressed;

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

    public void PressPauseButton()
    {
        if (OnPauseButtonPressed == null)
            return;
        
        OnPauseButtonPressed();
    }
}
