using Enemies;
using Player;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes
{
    public interface ISceneVM
    {
        void PauseGame(bool paused);
        void TransitionTo(SceneState sceneState);
    }

    public class SceneVM: ISceneVM
    {
        private SceneState SceneState {
            get => sceneState;
            set {
                sceneState = value;
                sceneState.BroadcastSceneState(sceneEvents);
            }
        }

        private IPlayerEvents playerEvents;
        private IEnemyEvents enemyEvents;
        private ISceneEvents sceneEvents;
        private SceneState sceneState;

        public SceneVM(
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
            this.sceneState.SetSceneVM(this);
        }

        public void PauseGame(bool paused)
        {
            Time.timeScale = paused ? 0f : 1f;
        }

        void OnDialogToggled(IDialogVM dialogVM)
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