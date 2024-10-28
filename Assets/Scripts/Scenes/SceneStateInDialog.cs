namespace Scenes
{
    public class SceneStateInDialog : SceneState
    {
        public override void BroadcastSceneState(ISceneEvents sceneEvents) {}

        public override void ToggleDialogOpen()
        {
            SceneViewModel.PauseGame(false);
            SceneViewModel.TransitionTo(new SceneStateIdle());
        }

        public override void ToggleGamePaused() {}
    }
}