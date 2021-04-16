using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerEvents : MonoBehaviour
{
    public event Action<Transform, List<IPlayerAbility>> OnSendPlayerLocation;
    public event Action OnRemovePlayerLocation;
    public event Action<IPlayerAbility> OnAbilityExecuted;

    public void SendPlayerLocation(Transform playerTransform, List<IPlayerAbility> abilities) {
        if (OnSendPlayerLocation == null) {
            return;
        }
        OnSendPlayerLocation(playerTransform, abilities);
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
