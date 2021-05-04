using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneEvents : MonoBehaviour {
    public event Action<DialogVM> OnDialogOpened;
    public event Action<DialogVM> OnDialogClosed;

    public void DialogOpened(DialogVM dialogVM) {
        if (OnDialogOpened == null)
            return;
        
        OnDialogOpened(dialogVM);
    }

    public void DialogClosed(DialogVM dialogVM) {
        if (OnDialogClosed == null)
            return;
        
        OnDialogClosed(dialogVM);
    }
}
