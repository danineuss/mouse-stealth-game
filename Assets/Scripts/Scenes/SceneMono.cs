using UnityEngine;

public class SceneMono: MonoBehaviour
{
    [SerializeField] private EventsMono eventsMono;
    [SerializeField] private PlayerMono playerMono;
    [SerializeField] private string sceneName;
    public ISceneVM SceneVM { get; private set; }

    void Awake()
    {
        SceneVM = new SceneVM(
            playerMono.PlayerVM,
            eventsMono.PlayerEvents, 
            eventsMono.EnemyEvents, 
            eventsMono.SceneEvents, 
            new SceneStateIdle(), 
            sceneName
        );
    }
}