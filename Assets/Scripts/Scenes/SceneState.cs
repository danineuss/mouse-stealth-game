namespace Scenes
{
    public abstract class SceneState
    {
        protected ISceneVM sceneVM;

        public void SetSceneVM(ISceneVM sceneVM)
        {
            this.sceneVM = sceneVM;
        }
        public abstract void BroadcastSceneState(ISceneEvents sceneEvents);
        public abstract void ToggleDialogOpen();
        public abstract void ToggleGamePaused();
    }
}