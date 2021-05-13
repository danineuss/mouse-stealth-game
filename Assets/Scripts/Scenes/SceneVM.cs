using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneVM
{
    void TransitionTo(SceneState sceneState);
}

public class SceneVM: ISceneVM
{
    private SceneState SceneState {
        get => sceneState;
        set {
            sceneState = value;
            sceneState.BroadcastSceneState(sceneEvents);
        }
    }

    private IPlayerEvents playerEvents;
    private IEnemyEvents enemyEvents;
    private ISceneEvents sceneEvents;
    private SceneState sceneState;

    public SceneVM(
        IPlayerEvents playerEvents,
        IEnemyEvents enemyEvents, 
        ISceneEvents sceneEvents)
    {
        this.playerEvents = playerEvents;
        this.enemyEvents = enemyEvents;
        this.sceneEvents = sceneEvents;
        TransitionTo(new SceneStateIdle());

        InitializeEvents();
    }

    void InitializeEvents()
    {
        sceneEvents.OnDialogOpened += ToggleDialogOpen;
        sceneEvents.OnDialogClosed += ToggleDialogOpen;
        sceneEvents.OnGameRestarted += RestartGame;
        playerEvents.OnPauseButtonPressed += HandlePauseButtonPressed;
        enemyEvents.OnGameFailed += FailGame;
    }

    public void TransitionTo(SceneState sceneState)
    {
        SceneState = sceneState;
        this.sceneState.SetSceneVM(this);
    }

    public void PauseGame(bool paused)
    {
        Time.timeScale = paused ? 0f : 1f;
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ToggleDialogOpen(IDialogVM dialogVM)
    {
        sceneState.ToggleDialogOpen();
    }

    void HandlePauseButtonPressed()
    {
        sceneState.ToggleGamePaused();
    }

    void FailGame()
    {
        TransitionTo(new SceneStateInFailed());
        sceneState.ToggleGamePaused();
    }
}
