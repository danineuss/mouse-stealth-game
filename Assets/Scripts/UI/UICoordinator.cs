using UnityEngine;

//TODO: make interface
public class UICoordinator : MonoBehaviour
{
    [SerializeField] private EventsMono eventsMono = null;
    [SerializeField] private GameObject failedScreen = null;
    [SerializeField] private GameObject pausedScreen = null;
    [SerializeField] private DialogMono introScreen = null;
    [SerializeField] private DialogMono distractAbilityScreen = null;
    [SerializeField] private DialogMono victoryScreen = null;
    [SerializeField] private GameObject headsUpDisplay = null;
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

    void OpenDialog(IDialogVM dialogVM) {
        dialogVM.SetActive(true);
        headsUpDisplay.SetActive(false);
    }

    void CloseDialog(IDialogVM dialogVM) {
        dialogVM.SetActive(false);
        headsUpDisplay.SetActive(true);
    }
}
