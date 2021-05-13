using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour {
    [SerializeField] private DialogVM dialogVM;
    [SerializeField] private EventsMono eventsMono;
    public bool DisableAfterDisplay = false;
    
    private Collider triggerCollider;
    
    void Awake() {
        triggerCollider = GetComponent<Collider>();
    }

    void OnTriggerEnter() {
        eventsMono.SceneEvents.DialogOpened(dialogVM);

        if (DisableAfterDisplay) {
            triggerCollider.gameObject.SetActive(false);
        }
    }
}
