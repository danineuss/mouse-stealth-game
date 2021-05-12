using System;

public interface ISceneEvents
{
    event Action<DialogVM> OnDialogOpened;
    event Action<DialogVM> OnDialogClosed;
    event Action OnGameRestarted;
    event Action<bool> OnGamePaused;
    event Action OnGameFailed;

    void DialogClosed(DialogVM dialogVM);
    void DialogOpened(DialogVM dialogVM);
    void RestartGame();
    void PauseGame(bool paused);
    void FailGame();
}

public class SceneEvents : ISceneEvents
{
    public event Action<DialogVM> OnDialogOpened;
    public event Action<DialogVM> OnDialogClosed;
    public event Action OnGameRestarted;
    public event Action<bool> OnGamePaused;
    public event Action OnGameFailed;

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

    public void RestartGame()
    {
        if (OnGameRestarted == null)
            return;
        
        OnGameRestarted();
    }

    public void PauseGame(bool paused)
    {
        if (OnGamePaused == null)
            return;
        
        OnGamePaused(paused);
    }

    public void FailGame()
    {
        if (OnGameFailed == null)
            return;
        
        OnGameFailed();
    }
}
