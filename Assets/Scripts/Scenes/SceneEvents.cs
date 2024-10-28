using System;
using UI;

namespace Scenes
{
    public interface ISceneEvents
    {
        event Action<IDialogViewModel> OnDialogOpened;
        event Action<IDialogViewModel> OnDialogClosed;
        event Action OnGameRestarted;
        event Action<bool> OnGamePaused;

        void CloseDialog(IDialogViewModel dialogViewModel);
        void OpenDialog(IDialogViewModel dialogViewModel);
        void RestartGame();
        void PauseGame(bool paused);
    }

    public class SceneEvents : ISceneEvents
    {
        public event Action<IDialogViewModel> OnDialogOpened;
        public event Action<IDialogViewModel> OnDialogClosed;
        public event Action OnGameRestarted;
        public event Action<bool> OnGamePaused;

        public void OpenDialog(IDialogViewModel dialogViewModel)
        {
            if (OnDialogOpened == null)
                return;

            OnDialogOpened(dialogViewModel);
        }

        public void CloseDialog(IDialogViewModel dialogViewModel)
        {
            if (OnDialogClosed == null)
                return;

            OnDialogClosed(dialogViewModel);
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
}