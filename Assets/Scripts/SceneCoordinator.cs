using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneState {
    Idle,
    Paused,
    Failed
}

public class SceneCoordinator : MonoBehaviour
{
    public List<PlayerDetector> PlayerDetectors;
    public UICoordinator UICoordinator;
    public FirstPersonCameraController FirstPersonCameraController;

    private SceneState sceneState;
    private float timeSinceLastPause;

    void Start() {
        sceneState = SceneState.Idle;
        timeSinceLastPause = Time.time;
        Time.timeScale = 1f;
    }

    void Update()
    {
        CheckPlayerDetection();
        CheckGamePaused();
        UpdateUI();
    }

    void CheckPlayerDetection() {
        foreach (var playerDetector in PlayerDetectors) {
            if (playerDetector.DetectorState.Equals(DetectorState.Alarmed)) {
                sceneState = SceneState.Failed;
                ToggleGamePaused();
                return;
            }
        }
    }

    void CheckGamePaused() {
        if (sceneState == SceneState.Failed || Time.unscaledTime - timeSinceLastPause < 0.2f) { 
            return; 
        }

        if (Input.GetKey(KeyCode.Escape)) {
            timeSinceLastPause = Time.unscaledTime;
            sceneState = (sceneState == SceneState.Idle) ? SceneState.Paused : SceneState.Idle;
            ToggleGamePaused();
        }
    }

    void ToggleGamePaused() {
        if (sceneState == SceneState.Idle) {
            Time.timeScale = 1f;
            FirstPersonCameraController.ToggleCursorLocked(true);
        } else {
            Time.timeScale = 0f;
            FirstPersonCameraController.ToggleCursorLocked(false);
        }
    }

    void UpdateUI() {
        switch (sceneState)
        {
            case SceneState.Idle:
                UICoordinator.ShowGamePaused(false);
                break;
            case SceneState.Paused:
                UICoordinator.ShowGamePaused(true);
                break;
            case SceneState.Failed:
                UICoordinator.ShowGameFailed();
                break;
            default:
                throw new InvalidOperationException("Switch case not exhaustive, code shoud not reach here.");
        }
    }

    public void RestartGame() {
        SceneManager.LoadScene("Test Scene");
    }
}
