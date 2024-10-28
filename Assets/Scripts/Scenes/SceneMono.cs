using Infrastructure;
using UnityEngine;

namespace Scenes
{
    public class SceneMono: MonoBehaviour
    {
        [SerializeField] private EventsMono eventsMono;

        private ISceneViewModel sceneViewModel;

        void Awake()
        {
            sceneViewModel = new SceneViewModel(
                eventsMono.PlayerEvents, 
                eventsMono.EnemyEvents, 
                eventsMono.SceneEvents
            );
        }
    }
}