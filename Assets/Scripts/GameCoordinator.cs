using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState {
    Idle,
    Paused,
    Failed
}

public class GameCoordinator : MonoBehaviour
{
    public List<PlayerDetector> PlayerDetectors;
    public UICoordinator UICoordinator;
    public FirstPersonCameraController FirstPersonCameraController;

    private GameState gameState;
    private float timeSinceLastPause;

    void Start() {
        gameState = GameState.Idle;
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
            if (playerDetector.CurrentDetectorState.Equals(DetectorState.Alarmed)) {
                gameState = GameState.Failed;
                ToggleGamePaused();
                return;
            }
        }
    }

    void CheckGamePaused() {
        if (gameState == GameState.Failed || Time.unscaledTime - timeSinceLastPause < 0.2f) { 
            return; 
        }

        if (Input.GetKey(KeyCode.Escape)) {
            timeSinceLastPause = Time.unscaledTime;
            gameState = (gameState == GameState.Idle) ? GameState.Paused : GameState.Idle;
            ToggleGamePaused();
        }
    }

    void ToggleGamePaused() {
        if (gameState == GameState.Idle) {
            Time.timeScale = 1f;
            FirstPersonCameraController.ToggleCursorLocked(true);
        } else {
            Time.timeScale = 0f;
            FirstPersonCameraController.ToggleCursorLocked(false);
        }
    }

    void UpdateUI() {
        switch (gameState)
        {
            case GameState.Idle:
                UICoordinator.ShowGamePaused(false);
                break;
            case GameState.Paused:
                UICoordinator.ShowGamePaused(true);
                break;
            case GameState.Failed:
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
