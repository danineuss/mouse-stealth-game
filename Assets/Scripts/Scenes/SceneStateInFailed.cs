namespace Scenes
{
    public class SceneStateInFailed : SceneState
    {
        public override void BroadcastSceneState(ISceneEvents sceneEvents) {}

        public override void ToggleDialogOpen() {}

        public override void ToggleGamePaused()
        {
            SceneViewModel.PauseGame(true);
        }
    }
}