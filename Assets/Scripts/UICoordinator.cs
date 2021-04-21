using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UICoordinator : MonoBehaviour
{
    [SerializeField] private SceneCoordinator sceneCoordinator;
    [SerializeField] private GameObject failedScreen;
    [SerializeField] private GameObject pausedScreen;
    [SerializeField] private GameObject introScreen;
    [SerializeField] private GameObject distractAbilityScreen;
    [SerializeField] private GameObject victoryScreen;


    void Start() {
        InitializeScreens();
    }

    void InitializeScreens() {
        failedScreen.SetActive(false);
        pausedScreen.SetActive(false);
        // introScreen.SetActive(true);
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

    public void CloseIntroScreen() {
        introScreen.SetActive(false);
        sceneCoordinator.UnpauseGame();
    }
}
