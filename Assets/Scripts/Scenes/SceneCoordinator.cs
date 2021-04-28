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

public class SceneCoordinator : MonoBehaviour {
    [SerializeField] private UICoordinator UICoordinator;
    [SerializeField] private FirstPersonCameraController FirstPersonCameraController;
    [SerializeField] private EnemyEvents enemyEvents;
    [SerializeField] private SceneEvents sceneEvents;
    [SerializeField] private string sceneName;

    private SceneState sceneState;
    private float timeSinceLastPause;

    public void RestartGame() {
        SceneManager.LoadScene(sceneName);
    }

    public void UnpauseGame() {
        ChangeGamePausedState(false);
    }

    void Start() {
        enemyEvents.OnDetectorChangedState += CheckForFailedGame;

        sceneState = SceneState.Idle;
        timeSinceLastPause = Time.time;

        ChangeGamePausedState(true);
    }

    void Update() {
        CheckGamePaused();
        UpdateUI();
    }

    void CheckForFailedGame(PlayerDetector playerDetector) {
        if (playerDetector.DetectorState == DetectorState.Alarmed) {
            sceneState = SceneState.Failed;
            ChangeGamePausedState(true);
        }
    }

    void CheckGamePaused() {
        if (sceneState == SceneState.Failed || Time.unscaledTime - timeSinceLastPause < 0.2f) { 
            return; 
        }        

        if (Input.GetKey(KeyCode.Escape)) {
            timeSinceLastPause = Time.unscaledTime;
            sceneState = (sceneState == SceneState.Idle) ? SceneState.Paused : SceneState.Idle;
            ChangeGamePausedState(sceneState == SceneState.Paused);
        }
    }

    void ChangeGamePausedState(bool paused) {
        if (paused) {
            Time.timeScale = 0f;
            FirstPersonCameraController.ChangeCursorLockedState(false);
        } else {
            Time.timeScale = 1f;
            FirstPersonCameraController.ChangeCursorLockedState(true);
        }
    }

    void UpdateUI() {
        switch (sceneState) {
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
}
