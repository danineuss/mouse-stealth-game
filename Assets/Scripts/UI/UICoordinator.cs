using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UICoordinator : MonoBehaviour
{
    [SerializeField] private SceneVM sceneVM;
    [SerializeField] private GameObject failedScreen;
    [SerializeField] private GameObject pausedScreen;
    [SerializeField] private DialogVM introScreen;
    [SerializeField] private DialogVM distractAbilityScreen;
    [SerializeField] private GameObject victoryScreen;
    public SceneEvents SceneEvents {
        get => sceneVM.SceneEvents;
    }

    void Start() {
        InitializeScreens();
        InitializeEvents();
    }

    void InitializeScreens() {
        failedScreen.SetActive(false);
        pausedScreen.SetActive(false);
        distractAbilityScreen.gameObject.SetActive(false);
        introScreen.gameObject.SetActive(true);
    }

    void InitializeEvents() {
        sceneVM.SceneEvents.OnDialogOpened += OpenDialog;
        sceneVM.SceneEvents.OnDialogClosed += CloseDialog;
    }

    public void ShowGamePaused(bool paused) {
        pausedScreen.SetActive(paused);
    }

    public void ShowGameFailed() {
        failedScreen.SetActive(true);
    }

    public void SendRestartGameCommand() {
        sceneVM.RestartGame();
    }

    void OpenDialog(DialogVM dialogVM) {
        dialogVM.gameObject.SetActive(true);
    }

    void CloseDialog(DialogVM dialogVM) {
        dialogVM.gameObject.SetActive(false);
    }
}
