namespace Scenes
{
    public class SceneStateIdle : SceneState
    {
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
}