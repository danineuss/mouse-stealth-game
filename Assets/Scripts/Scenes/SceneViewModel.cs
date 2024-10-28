using Enemies;
using Player;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes
{
    public interface ISceneViewModel
    {
        void PauseGame(bool paused);
        void TransitionTo(SceneState sceneState);
    }

    public class SceneViewModel: ISceneViewModel
    {
        private SceneState SceneState {
            get => sceneState;
            set {
                sceneState = value;
                sceneState.BroadcastSceneState(sceneEvents);
            }
        }

        private SceneState sceneState;
        
        private readonly IPlayerEvents playerEvents;
        private readonly IEnemyEvents enemyEvents;
        private readonly ISceneEvents sceneEvents;

        public SceneViewModel(
            IPlayerEvents playerEvents,
            IEnemyEvents enemyEvents, 
            ISceneEvents sceneEvents)
        {
            this.playerEvents = playerEvents;
            this.enemyEvents = enemyEvents;
            this.sceneEvents = sceneEvents;
            TransitionTo(new SceneStateIdle());

            InitializeEvents();
        }

        void InitializeEvents()
        {
            sceneEvents.OnDialogOpened += OnDialogToggled;
            sceneEvents.OnDialogClosed += OnDialogToggled;
            sceneEvents.OnGameRestarted += OnGameRestarted;

            playerEvents.OnPauseButtonPressed += OnPauseButtonPressed;
            playerEvents.OnCharacterPanicked += OnCharacterPanicked;
        
            enemyEvents.OnGameFailed += FailGame;
        }

        public void TransitionTo(SceneState sceneState)
        {
            SceneState = sceneState;
            this.sceneState.SetSceneViewModel(this);
        }

        public void PauseGame(bool paused)
        {
            Time.timeScale = paused ? 0f : 1f;
        }

        void OnDialogToggled(IDialogViewModel dialogViewModel)
        {
            sceneState.ToggleDialogOpen();
        }

        void OnGameRestarted()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnCharacterPanicked()
        {
            enemyEvents.FailGame();
        }

        void OnPauseButtonPressed()
        {
            sceneState.ToggleGamePaused();
        }

        void FailGame()
        {
            TransitionTo(new SceneStateInFailed());
            sceneState.ToggleGamePaused();
        }
    }
}