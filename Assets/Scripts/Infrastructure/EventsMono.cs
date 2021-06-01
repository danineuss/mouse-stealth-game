using System.Collections;
using UnityEngine;

public interface IEvents
{
    IPlayerEvents PlayerEvents { get; }
    IEnemyEvents EnemyEvents { get; }
    ISceneEvents SceneEvents { get; }

    void StartCoroutine(IEnumerator coroutine);
    void StopCoroutine(IEnumerator coroutine);
}

public class EventsMono : MonoBehaviour, IEvents
{
    public IPlayerEvents PlayerEvents => playerEvents;
    public IEnemyEvents EnemyEvents => enemyEvents;
    public ISceneEvents SceneEvents => sceneEvents;
    private IPlayerEvents playerEvents;
    private IEnemyEvents enemyEvents;
    private ISceneEvents sceneEvents;

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
