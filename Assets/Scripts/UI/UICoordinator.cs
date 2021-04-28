using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UICoordinator : MonoBehaviour
{
    [SerializeField] private SceneCoordinator sceneCoordinator;
    [SerializeField] private GameObject failedScreen;
    [SerializeField] private GameObject pausedScreen;
    [SerializeField] private DialogVM introScreen;
    [SerializeField] private DialogVM distractAbilityScreen;
    [SerializeField] private GameObject victoryScreen;


    void Start() {
        InitializeScreens();
    }

    void InitializeScreens() {
        failedScreen.SetActive(false);
        pausedScreen.SetActive(false);
        distractAbilityScreen.gameObject.SetActive(false);
        introScreen.gameObject.SetActive(true);
    }

    public void ShowGamePaused(bool paused) {
        pausedScreen.SetActive(paused);
    }

    public void ShowGameFailed() {
        failedScreen.SetActive(true);
    }

    public void SendRestartGameCommand() {
        sceneCoordinator.RestartGame();
    }

    public void OpenDialog(DialogVM dialogVM) {
        dialogVM.gameObject.SetActive(true);
        sceneCoordinator.ChangeGamePausedState(true);
    }

    public void CloseDialog(DialogVM dialogVM) {
        dialogVM.gameObject.SetActive(false);
        sceneCoordinator.ChangeGamePausedState(false);
    }
}
