using Infrastructure;
using Scenes;
using UnityEngine;

//TODO: make interface
namespace UI
{
    public class UICoordinator : MonoBehaviour
    {
        [SerializeField] private EventsMono eventsMono;
        [SerializeField] private GameObject failedScreen;
        [SerializeField] private GameObject pausedScreen;
        [SerializeField] private DialogMono introScreen;
        [SerializeField] private DialogMono distractAbilityScreen;
        [SerializeField] private DialogMono victoryScreen;
        [SerializeField] private GameObject headsUpDisplay;
        public ISceneEvents SceneEvents => eventsMono.SceneEvents;

        void Start() {
            InitializeScreens();
            InitializeEvents();
        }

        void InitializeScreens() {
            failedScreen.SetActive(false);
            pausedScreen.SetActive(false);
            introScreen.gameObject.SetActive(false);
            distractAbilityScreen.gameObject.SetActive(false);
            victoryScreen.gameObject.SetActive(false);
            headsUpDisplay.SetActive(false);
        }

        void InitializeEvents() {
            eventsMono.SceneEvents.OnDialogOpened += OpenDialog;
            eventsMono.SceneEvents.OnDialogClosed += CloseDialog;
            eventsMono.SceneEvents.OnGamePaused += ShowGamePaused;
            eventsMono.EnemyEvents.OnGameFailed += ShowGameFailed;
        }

        public void SendRestartGameCommand() {
            eventsMono.SceneEvents.RestartGame();
        }

        void ShowGamePaused(bool paused) {
            pausedScreen.SetActive(paused);
            headsUpDisplay.SetActive(!paused);
        }

        void ShowGameFailed() {
            failedScreen.SetActive(true);
            headsUpDisplay.SetActive(false);
        }

        void OpenDialog(IDialogViewModel dialogViewModel) {
            dialogViewModel.SetActive(true);
            headsUpDisplay.SetActive(false);
        }

        void CloseDialog(IDialogViewModel dialogViewModel) {
            dialogViewModel.SetActive(false);
            headsUpDisplay.SetActive(true);
        }
    }
}
