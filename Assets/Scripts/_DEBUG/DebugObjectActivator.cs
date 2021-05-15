using UnityEngine;

public class DebugObjectActivator : MonoBehaviour {
    public GameObject Object;
    public bool DesiredStateAfterTrigger;
    public EventsMono EventsMono;
    
    void OnTriggerEnter() {
        Object.SetActive(DesiredStateAfterTrigger);
        EventsMono.PlayerEvents.AbilityLearned(new PlayerAbilityDistract());
    }
}
