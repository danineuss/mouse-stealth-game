using System;
using UnityEngine;

public interface IPlayerEvents
{
    event Action<Guid, bool, Transform> OnSendPlayerLocation;
    event Action<Guid> OnRemovePlayerLocation;
    event Action<IPlayerAbility> OnAbilityExecuted;
    event Action<IPlayerAbility> OnAbilityLearned;
    event Action OnPauseButtonPressed;

    void AbilityExecuted(IPlayerAbility ability);
    void AbilityLearned(IPlayerAbility ability);
    void RemovePlayerLocation(Guid enemyID);
    void SendPlayerLocation(Guid enemyID, bool shouldDisplayText, Transform playerTransform);
    void PressPauseButton();
}

public class PlayerEvents : IPlayerEvents
{
    public event Action<Guid, bool, Transform> OnSendPlayerLocation;
    public event Action<Guid> OnRemovePlayerLocation;
    public event Action<IPlayerAbility> OnAbilityExecuted;
    public event Action<IPlayerAbility> OnAbilityLearned;
    public event Action OnPauseButtonPressed;

    public void SendPlayerLocation(
        Guid enemyID, bool shouldDisplayText, Transform playerTransform)
    {
        if (OnSendPlayerLocation == null)
            return;

        OnSendPlayerLocation(enemyID, shouldDisplayText, playerTransform);
    }

    public void RemovePlayerLocation(Guid enemyID)
    {
        if (OnRemovePlayerLocation == null)
            return;

        OnRemovePlayerLocation(enemyID);
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
