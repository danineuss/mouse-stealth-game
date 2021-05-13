using System;
using UnityEngine;

public abstract class SceneState
{
    protected SceneVM sceneVM;

    public void SetSceneVM(SceneVM sceneVM)
    {
        this.sceneVM = sceneVM;
    }
    public abstract bool EligibleForPausingGame { get; }
    public abstract void BroadcastSceneState(ISceneEvents sceneEvents);
    public abstract void ToggleDialogOpen();
    public abstract void ToggleGamePaused();
}

public class SceneStateIdle : SceneState
{
    public override bool EligibleForPausingGame => true;

    public override void BroadcastSceneState(ISceneEvents sceneEvents)
    {
        sceneEvents.PauseGame(false);
    }

    public override void ToggleDialogOpen()
    {
        sceneVM.PauseGame(true);
        sceneVM.TransitionTo(new SceneStateInDialog());
    }

    public override void ToggleGamePaused()
    {
        sceneVM.PauseGame(true);
        sceneVM.TransitionTo(new SceneStatePaused());
    }
}

public class SceneStatePaused : SceneState
{
    public override bool EligibleForPausingGame => true;

    public override void BroadcastSceneState(ISceneEvents sceneEvents)
    {
        sceneEvents.PauseGame(true);
    }

    public override void ToggleDialogOpen() {}

    public override void ToggleGamePaused()
    {
        sceneVM.PauseGame(false);
        sceneVM.TransitionTo(new SceneStateIdle());
    }
}

public class SceneStateInDialog : SceneState
{
    public override bool EligibleForPausingGame => false;

    public override void BroadcastSceneState(ISceneEvents sceneEvents) {}

    public override void ToggleDialogOpen()
    {
        sceneVM.PauseGame(false);
        sceneVM.TransitionTo(new SceneStateIdle());
    }

    public override void ToggleGamePaused() {}
}

public class SceneStateInFailed : SceneState
{
    public override bool EligibleForPausingGame => false;

    public override void BroadcastSceneState(ISceneEvents sceneEvents)
    {
        sceneEvents.FailGame();
    }

    public override void ToggleDialogOpen() {}

    public override void ToggleGamePaused()
    {
        sceneVM.PauseGame(true);
    }
}
