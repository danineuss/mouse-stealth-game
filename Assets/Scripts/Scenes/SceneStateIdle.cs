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
            SceneViewModel.PauseGame(true);
            SceneViewModel.TransitionTo(new SceneStateInDialog());
        }

        public override void ToggleGamePaused()
        {
            SceneViewModel.PauseGame(true);
            SceneViewModel.TransitionTo(new SceneStatePaused());
        }
    }
}