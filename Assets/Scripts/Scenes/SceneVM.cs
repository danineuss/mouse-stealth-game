using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneState {
    Idle,
    Paused,
    InDialog,
    Failed
}

public class SceneVM : MonoBehaviour {
    [SerializeField] private UICoordinator UICoordinator;
    [SerializeField] private EnemyEvents enemyEvents;
    [SerializeField] private SceneEvents sceneEvents;
    [SerializeField] private PlayerVM playerVM;
    [SerializeField] private string sceneName;
    public SceneEvents SceneEvents{
        get => sceneEvents;
        private set => sceneEvents = value;
    }

    private SceneState sceneState;
    private float timeSinceLastPause;

    public void RestartGame() {
        SceneManager.LoadScene(sceneName);
    }

    void Start() {
        InitializeEvents();

        sceneState = SceneState.InDialog;
        timeSinceLastPause = Time.time;

        ChangeGamePausedState(true);
    }

    void InitializeEvents() {
        enemyEvents.OnDetectorStateChanged += CheckForFailedGame;
        sceneEvents.OnDialogOpened += OpenDialog;
        sceneEvents.OnDialogClosed += CloseDialog;
    }

    void Update() {
        CheckGamePaused();
        UpdateUI();
    }

    void ChangeGamePausedState(bool paused) {
        if (paused) {
            Time.timeScale = 0f;
            playerVM.CameraController.ChangeCursorLockedState(false);
        } else {
            Time.timeScale = 1f;
            playerVM.CameraController.ChangeCursorLockedState(true);
        }
    }

    void CheckForFailedGame(PlayerDetector playerDetector) {
        if (playerDetector.DetectorState == DetectorState.Alarmed) {
            sceneState = SceneState.Failed;
            ChangeGamePausedState(true);
        }
    }

    void OpenDialog(DialogVM dialogVM) {
        sceneState = SceneState.InDialog;
        ChangeGamePausedState(true);
    }

    void CloseDialog(DialogVM dialogVM) {
        sceneState = SceneState.Idle;
        ChangeGamePausedState(false);
    }

    void CheckGamePaused() {
        if (sceneState == SceneState.Failed || 
            sceneState == SceneState.InDialog ||
            Time.unscaledTime - timeSinceLastPause < 0.2f) {
            return; 
        }

        if(playerVM.PlayerInput.GetKeyDown(PlayerInput.Escape)) {
            timeSinceLastPause = Time.unscaledTime;
            sceneState = (sceneState == SceneState.Idle) ? SceneState.Paused : SceneState.Idle;
            ChangeGamePausedState(sceneState == SceneState.Paused);
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
            case SceneState.InDialog:
                break;
            case SceneState.Failed:
                UICoordinator.ShowGameFailed();
                break;
            default:
                throw new InvalidOperationException("Switch case not exhaustive: " + sceneState);
        }
    }
}
