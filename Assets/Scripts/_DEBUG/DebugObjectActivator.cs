using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugObjectActivator : MonoBehaviour {
    public GameObject Object;
    public bool DesiredStateAfterTrigger;
    public PlayerEventsMono PlayerEventsMono;
    
    void OnTriggerEnter() {
        Object.SetActive(DesiredStateAfterTrigger);
        PlayerEventsMono.PlayerEvents.AbilityLearned(new PlayerAbilityDistract());
    }
}
