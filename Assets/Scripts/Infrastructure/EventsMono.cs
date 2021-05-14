using System.Collections;
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

    public new void StartCoroutine(IEnumerator coroutine)
    {
        base.StartCoroutine(coroutine);
    }

    public new void StopCoroutine(IEnumerator coroutine)
    {
        base.StopCoroutine(coroutine);
    }
}
