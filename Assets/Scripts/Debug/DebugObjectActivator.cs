using Infrastructure;
using Player;
using UnityEngine;

namespace Debug
{
    public class DebugObjectActivator : MonoBehaviour {
        public GameObject Object;
        public bool DesiredStateAfterTrigger;
        public EventsMono EventsMono;
        public float DistractionDuration;
    
        void OnTriggerEnter() {
            Object.SetActive(DesiredStateAfterTrigger);
            EventsMono.PlayerEvents.LearnAbility(new PlayerAbilityDistract(DistractionDuration));
        }
    }
}
