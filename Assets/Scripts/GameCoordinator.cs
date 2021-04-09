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
                Time.timeScale = 0.0f;
                return;
            }
        }
    }

    void CheckGamePaused() {

    }

    void UpdateUI() {
        switch (gameState)
        {
            case GameState.Idle:
                UICoordinator.HideGamePaused();
                break;
            case GameState.Paused:
                UICoordinator.ShowGamePaused();
                break;
            case GameState.Failed:
                UICoordinator.ShowGameFailed();
                break;
            default:
                throw new InvalidOperationException("Switch case not exhaustive, code shoud not reach here.");
        }
    }
}
