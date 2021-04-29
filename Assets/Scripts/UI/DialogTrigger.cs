using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour {
    [SerializeField] private DialogVM dialogVM;
    [SerializeField] private SceneVM sceneVM;
    
    void OnTriggerEnter() {
        sceneVM.SceneEvents.DialogOpened(dialogVM);
    }
}
