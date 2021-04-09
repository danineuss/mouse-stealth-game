using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UICoordinator : MonoBehaviour
{
    public GameCoordinator GameCoordinator;
    private GameObject failedScreen;
    private GameObject pausedScreen;

    void Start() {
        InitializeScreens();
    }

    void InitializeScreens() {
        failedScreen = GetComponentsInChildren<Transform>()
                        .Where(x => x.CompareTag("FailedScreen"))
                        .First()
                        .gameObject;
        pausedScreen = GetComponentsInChildren<Transform>()
                        .Where(x => x.CompareTag("PausedScreen"))
                        .First()
                        .gameObject;

        failedScreen.SetActive(false);
        pausedScreen.SetActive(false);
    }

    public void ShowGamePaused(bool paused) {
        pausedScreen.SetActive(paused);
    }

    public void ShowGameFailed() {
        failedScreen.SetActive(true);
    }

    public void SendRestartGameCommand() {
        GameCoordinator.RestartGame();
    }
}
