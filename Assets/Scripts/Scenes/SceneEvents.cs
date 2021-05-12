using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneEvents
{
    event Action<DialogVM> OnDialogOpened;
    event Action<DialogVM> OnDialogClosed;

    void DialogClosed(DialogVM dialogVM);
    void DialogOpened(DialogVM dialogVM);
}

public class SceneEvents : ISceneEvents
{
    public event Action<DialogVM> OnDialogOpened;
    public event Action<DialogVM> OnDialogClosed;

    public void DialogOpened(DialogVM dialogVM)
    {
        if (OnDialogOpened == null)
            return;

        OnDialogOpened(dialogVM);
    }

    public void DialogClosed(DialogVM dialogVM)
    {
        if (OnDialogClosed == null)
            return;

        OnDialogClosed(dialogVM);
    }
}
