using UnityEngine;

public class EventsMono : MonoBehaviour
{
    public PlayerEvents PlayerEvents => playerEvents;
    public EnemyEvents EnemyEvents => enemyEvents;
    public SceneEvents SceneEvents => sceneEvents;
    private PlayerEvents playerEvents;
    private EnemyEvents enemyEvents;
    private SceneEvents sceneEvents;

    void Awake()
    {
        playerEvents = new PlayerEvents();
        enemyEvents = new EnemyEvents();
        sceneEvents = new SceneEvents();
    }
}
