using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    Idle,
    Paused,
    Failed
}

public class GameCoordinator : MonoBehaviour
{
    public List<PlayerDetector> PlayerDetectors;
    public UICoordinator UICoordinator;

    private GameState gameState = GameState.Idle;
    private float timeSinceLastPause;

    void Start() {
        timeSinceLastPause = Time.time;
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
                Time.timeScale = 0f;
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
            ToggleGamePaused();
        }
    }

    void ToggleGamePaused() {
        if (gameState == GameState.Idle) {
            gameState = GameState.Paused;
            Time.timeScale = 0f;
        } else {
            gameState = GameState.Idle;
            Time.timeScale = 1f;
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
}
