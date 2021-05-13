using UnityEngine;

public class SceneMono: MonoBehaviour
{
    [SerializeField] private EventsMono eventsMono;
    [SerializeField] private PlayerMono playerMono;

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