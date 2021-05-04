using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugObjectActivator : MonoBehaviour {
    public GameObject Object;
    public bool DesiredStateAfterTrigger;
    public PlayerEvents PlayerEvents;
    
    void OnTriggerEnter() {
        Object.SetActive(DesiredStateAfterTrigger);
        PlayerEvents.AbilityLearned(new PlayerAbilityDistract());
    }
}
