namespace Scenes
{
    public class SceneStateInDialog : SceneState
    {
        public override void BroadcastSceneState(ISceneEvents sceneEvents) {}

        public override void ToggleDialogOpen()
        {
            sceneVM.PauseGame(false);
            sceneVM.TransitionTo(new SceneStateIdle());
        }

        public override void ToggleGamePaused() {}
    }
}