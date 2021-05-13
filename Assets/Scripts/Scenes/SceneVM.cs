using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneVM
{
    ISceneEvents SceneEvents { get; }
    void TransitionTo(SceneState sceneState);
}

public class SceneVM: ISceneVM
{
    public ISceneEvents SceneEvents => sceneEvents;
    public SceneState SceneState {
        get => sceneState;
        private set {
            sceneState = value;
            sceneState.BroadcastSceneState(sceneEvents);
        }
    }

    private IPlayerVM playerVM;
    private IPlayerEvents playerEvents;
    private IEnemyEvents enemyEvents;
    private ISceneEvents sceneEvents;
    private SceneState sceneState;
    private string sceneName;

    public SceneVM(
        IPlayerVM playerVM, 
        IPlayerEvents playerEvents,
        IEnemyEvents enemyEvents, 
        ISceneEvents sceneEvents, 
        SceneState sceneState, 
        string sceneName)
    {
        this.playerVM = playerVM;
        this.playerEvents = playerEvents;
        this.enemyEvents = enemyEvents;
        this.sceneEvents = sceneEvents;
        this.sceneName = sceneName;
        TransitionTo(sceneState);

        InitializeEvents();
    }

    void InitializeEvents()
    {
        sceneEvents.OnDialogOpened += ToggleDialogOpen;
        sceneEvents.OnDialogClosed += ToggleDialogOpen;
        sceneEvents.OnGameRestarted += RestartGame;
        playerEvents.OnPauseButtonPressed += HandlePauseButtonPressed;
        enemyEvents.OnDetectorStateChanged += CheckForFailedGame;
    }

    public void TransitionTo(SceneState sceneState)
    {
        SceneState = sceneState;
        this.sceneState.SetSceneVM(this);
    }

    public void PauseGame(bool paused)
    {
        if (paused)
        {
            Time.timeScale = 0f;
            playerVM.LockCursor(false);
        }
        else
        {
            Time.timeScale = 1f;
            playerVM.LockCursor(true);
        }
    }

    void CheckForFailedGame(PlayerDetector playerDetector)
    {
        if (playerDetector.DetectorState == DetectorState.Alarmed)
        {
            TransitionTo(new SceneStateInFailed());
            sceneState.ToggleGamePaused();
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene(sceneName);
    }

    void ToggleDialogOpen(DialogVM dialogVM)
    {
        sceneState.ToggleDialogOpen();
    }

    void HandlePauseButtonPressed()
    {
        if (!sceneState.EligibleForPausingGame)
            return;

        sceneState.ToggleGamePaused();
    }
}
