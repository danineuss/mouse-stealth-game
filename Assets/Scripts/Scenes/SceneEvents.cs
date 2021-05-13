using System;

public interface ISceneEvents
{
    event Action<IDialogVM> OnDialogOpened;
    event Action<IDialogVM> OnDialogClosed;
    event Action OnGameRestarted;
    event Action<bool> OnGamePaused;

    void CloseDialog(IDialogVM dialogVM);
    void OpenDialog(IDialogVM dialogVM);
    void RestartGame();
    void PauseGame(bool paused);
}

public class SceneEvents : ISceneEvents
{
    public event Action<IDialogVM> OnDialogOpened;
    public event Action<IDialogVM> OnDialogClosed;
    public event Action OnGameRestarted;
    public event Action<bool> OnGamePaused;

    public void OpenDialog(IDialogVM dialogVM)
    {
        if (OnDialogOpened == null)
            return;

        OnDialogOpened(dialogVM);
    }

    public void CloseDialog(IDialogVM dialogVM)
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
}
