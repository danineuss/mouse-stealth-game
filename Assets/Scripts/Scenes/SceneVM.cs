using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneStateEnum {
    Idle,
    Paused,
    InDialog,
    Failed
}

public abstract class SceneState
{
    protected SceneVM sceneVM;

    public void SetSceneVM(SceneVM sceneVM)
    {
        this.sceneVM = sceneVM;
    }

    public abstract void UpdateGamePausedState();
}

public class SceneStateIdle : SceneState
{
    public override void UpdateGamePausedState()
    {
        throw new NotImplementedException();
    }
}

public class SceneStateInDialog : SceneState
{
    public override void UpdateGamePausedState()
    {
        throw new NotImplementedException();
    }
}

public interface ISceneVM
{
    ISceneEvents SceneEvents { get; }
    void TransitionTo(SceneState sceneState);
    void Update();
}

public class SceneVM: ISceneVM
{
    public ISceneEvents SceneEvents => sceneEvents;

    private SceneStateEnum sceneStateEnum;
    private float timeSinceLastPause;
    private IPlayerVM playerVM;
    private IEnemyEvents enemyEvents;
    private ISceneEvents sceneEvents;
    private SceneState sceneState;
    private string sceneName;

    public SceneVM(
        IPlayerVM playerVM, 
        IEnemyEvents enemyEvents, 
        ISceneEvents sceneEvents, 
        SceneState sceneState, 
        string sceneName)
    {
        this.playerVM = playerVM;
        this.enemyEvents = enemyEvents;
        this.sceneEvents = sceneEvents;
        this.sceneState = sceneState;
        this.sceneName = sceneName;

        InitializeEvents();

        sceneStateEnum = SceneStateEnum.InDialog;
        timeSinceLastPause = Time.time;

        ChangeGamePausedState(true);
    }

    void InitializeEvents()
    {
        enemyEvents.OnDetectorStateChanged += CheckForFailedGame;
        sceneEvents.OnDialogOpened += OpenDialog;
        sceneEvents.OnDialogClosed += CloseDialog;
        sceneEvents.OnGameRestarted += RestartGame;
    }


    public void TransitionTo(SceneState sceneState)
    {

    }

    public void Update()
    {
        CheckGamePaused();
        UpdateUI();
    }

    void ChangeGamePausedState(bool paused)
    {
        if (paused)
        {
            Time.timeScale = 0f;
            playerVM.ChangeCursorLockedState(false);
        }
        else
        {
            Time.timeScale = 1f;
            playerVM.ChangeCursorLockedState(true);
        }
    }

    void CheckForFailedGame(PlayerDetector playerDetector)
    {
        if (playerDetector.DetectorState == DetectorState.Alarmed)
        {
            sceneStateEnum = SceneStateEnum.Failed;
            ChangeGamePausedState(true);
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene(sceneName);
    }

    void OpenDialog(DialogVM dialogVM)
    {
        sceneStateEnum = SceneStateEnum.InDialog;
        ChangeGamePausedState(true);
    }

    void CloseDialog(DialogVM dialogVM)
    {
        sceneStateEnum = SceneStateEnum.Idle;
        ChangeGamePausedState(false);
    }

    void CheckGamePaused()
    {
        if (sceneStateEnum == SceneStateEnum.Failed ||
            sceneStateEnum == SceneStateEnum.InDialog ||
            Time.unscaledTime - timeSinceLastPause < 0.2f)
        {
            return;
        }

        if (playerVM.PlayerInput.GetKeyDown(PlayerInput.Escape))
        {
            timeSinceLastPause = Time.unscaledTime;
            sceneStateEnum = (sceneStateEnum == SceneStateEnum.Idle) ? SceneStateEnum.Paused : SceneStateEnum.Idle;
            ChangeGamePausedState(sceneStateEnum == SceneStateEnum.Paused);
        }
    }

    void UpdateUI()
    {
        switch (sceneStateEnum)
        {
            case SceneStateEnum.Idle:
                sceneEvents.PauseGame(false);
                break;
            case SceneStateEnum.Paused:
                sceneEvents.PauseGame(true);
                break;
            case SceneStateEnum.InDialog:
                break;
            case SceneStateEnum.Failed:
                sceneEvents.FailGame();
                break;
            default:
                throw new InvalidOperationException("Switch case not exhaustive: " + sceneStateEnum);
        }
    }
}
