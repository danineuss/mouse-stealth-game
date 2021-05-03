using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour {
    private Collider triggerCollider;
    [SerializeField] private DialogVM dialogVM;
    [SerializeField] private SceneVM sceneVM;
    
    void Awake() {
        triggerCollider = GetComponent<Collider>();
    }

    void OnTriggerEnter() {
        sceneVM.SceneEvents.DialogOpened(dialogVM);
        triggerCollider.gameObject.SetActive(false);
    }
}
