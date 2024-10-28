namespace Scenes
{
    public abstract class SceneState
    {
        protected ISceneViewModel SceneViewModel;

        public void SetSceneViewModel(ISceneViewModel sceneViewModel)
        {
            SceneViewModel = sceneViewModel;
        }
        
        public abstract void BroadcastSceneState(ISceneEvents sceneEvents);
        public abstract void ToggleDialogOpen();
        public abstract void ToggleGamePaused();
    }
}