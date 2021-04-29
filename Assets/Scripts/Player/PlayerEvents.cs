using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerEvents : MonoBehaviour {
    public event Action<EnemyVM, bool, Transform> OnSendPlayerLocation;
    public event Action<EnemyVM> OnRemovePlayerLocation;
    public event Action<IPlayerAbility> OnAbilityExecuted;
    public event Action<IPlayerAbility> OnAbilityLearned;

    public void SendPlayerLocation(
        EnemyVM enemyVM, bool shouldDisplayText, Transform playerTransform
    ) {
        if (OnSendPlayerLocation == null)
            return;

        OnSendPlayerLocation(enemyVM, shouldDisplayText, playerTransform);
    }
    
    public void RemovePlayerLocation(EnemyVM enemyVM) {
        if (OnRemovePlayerLocation == null)
            return;
        
        OnRemovePlayerLocation(enemyVM);
    }

    public void AbilityExecuted(IPlayerAbility ability) {
        if (OnAbilityExecuted == null)
            return;
        
        OnAbilityExecuted(ability);
    }

    public void AbilityLearned(IPlayerAbility ability) {
        if (OnAbilityLearned == null)
            return;
        
        OnAbilityLearned(ability);
    }
}
