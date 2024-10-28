namespace Scenes
{
    public class SceneStatePaused : SceneState
    {
        public override void BroadcastSceneState(ISceneEvents sceneEvents)
        {
            sceneEvents.PauseGame(true);
        }

        public override void ToggleDialogOpen() {}

        public override void ToggleGamePaused()
        {
            SceneViewModel.PauseGame(false);
            SceneViewModel.TransitionTo(new SceneStateIdle());
        }
    }
}