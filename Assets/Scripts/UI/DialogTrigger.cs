using Infrastructure;
using UnityEngine;

namespace UI
{
    public class DialogTrigger : MonoBehaviour 
    {
        [SerializeField] private DialogMono dialogMono;
        [SerializeField] private EventsMono eventsMono;
        [SerializeField] private bool disableAfterDisplay;
        [SerializeField] private bool triggerUponStart;
    
        private Collider triggerCollider;
    
        void Awake() 
        {
            triggerCollider = GetComponent<Collider>();
        }

        void Start()
        {
            if (triggerUponStart)
                TriggerDialog();
        }
        void OnTriggerEnter() 
        {
            if (!triggerUponStart)
                TriggerDialog();
        }

        void TriggerDialog()
        {
            eventsMono.SceneEvents.OpenDialog(dialogMono.DialogViewModel);

            if (disableAfterDisplay)
                triggerCollider.gameObject.SetActive(false);
        }
    }
}
