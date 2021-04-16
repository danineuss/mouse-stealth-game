using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerEvents : MonoBehaviour
{
    public event Action<EnemyVM, bool, Transform> OnSendPlayerLocation;
    public event Action OnRemovePlayerLocation;
    public event Action<IPlayerAbility> OnAbilityExecuted;

    public void SendPlayerLocation(
        EnemyVM enemyVM, bool shouldDisplayText, Transform playerTransform
    ) {
        if (OnSendPlayerLocation == null) {
            return;
        }
        OnSendPlayerLocation(enemyVM, shouldDisplayText, playerTransform);
    }
    
    public void RemovePlayerLocation() {
        if (OnRemovePlayerLocation == null) {
            return;
        }
        OnRemovePlayerLocation();
    }

    public void AbilityExecuted(IPlayerAbility ability) {
        if (OnAbilityExecuted == null) {
            return;
        }
        OnAbilityExecuted(ability);
    }
}
