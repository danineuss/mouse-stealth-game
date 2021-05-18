using System;
using UnityEngine;

public interface IPlayerEvents
{
    event Action<IPlayerAbility> OnAbilityExecuted;
    event Action<IPlayerAbility> OnAbilityLearned;
    event Action<Guid> OnEnemyDistracted;
    event Action OnPauseButtonPressed;
    event Action<Guid> OnPlayerLocationRemoved;
    event Action<Guid, bool, Transform> OnPlayerLocationSent;

    void DistractEnemy(Guid enemyID);
    void ExecuteAbility(IPlayerAbility ability);
    void LearnAbility(IPlayerAbility ability);
    void PressPauseButton();
    void RemovePlayerLocation(Guid enemyID);
    void SendPlayerLocation(Guid enemyID, bool shouldDisplayText, Transform playerTransform);
}

public class PlayerEvents : IPlayerEvents
{
    public event Action<IPlayerAbility> OnAbilityExecuted;
    public event Action<IPlayerAbility> OnAbilityLearned;
    public event Action<Guid> OnEnemyDistracted;
    public event Action OnPauseButtonPressed;
    public event Action<Guid> OnPlayerLocationRemoved;
    public event Action<Guid, bool, Transform> OnPlayerLocationSent;

    public void DistractEnemy(Guid enemyID)
    {
        if (OnEnemyDistracted == null)
            return;
        
        OnEnemyDistracted(enemyID);
    }

    public void ExecuteAbility(IPlayerAbility ability)
    {
        if (OnAbilityExecuted == null)
            return;

        OnAbilityExecuted(ability);
    }

    public void LearnAbility(IPlayerAbility ability)
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

    public void RemovePlayerLocation(Guid enemyID)
    {
        if (OnPlayerLocationRemoved == null)
            return;

        OnPlayerLocationRemoved(enemyID);
    }

    public void SendPlayerLocation(
        Guid enemyID, bool shouldDisplayText, Transform playerTransform)
    {
        if (OnPlayerLocationSent == null)
            return;

        OnPlayerLocationSent(enemyID, shouldDisplayText, playerTransform);
    }
}
