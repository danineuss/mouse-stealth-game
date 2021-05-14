using UnityEngine;

public class SceneMono: MonoBehaviour
{
    [SerializeField] private EventsMono eventsMono = null;

    private ISceneVM sceneVM;

    void Awake()
    {
        sceneVM = new SceneVM(
            eventsMono.PlayerEvents, 
            eventsMono.EnemyEvents, 
            eventsMono.SceneEvents
        );
    }
}