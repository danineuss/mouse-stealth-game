using NSubstitute;
using System.Collections;
using UnityEngine;

public class EventsMono_Mock : IEvents
{
    public IPlayerEvents PlayerEvents { get; set; }
    public IEnemyEvents EnemyEvents { get; set; }
    public ISceneEvents SceneEvents { get; set; }
    public CoroutineTestRunner CoroutineTestRunner { get; set; }
    public int CoroutineStartCounter { get; set; }
    public int CoroutineStopCounter { get; set; }

    private GameObject gameObject;

    public EventsMono_Mock()
    {
        gameObject = new GameObject("CoroutineTestRunner");
        this.CoroutineTestRunner = gameObject.AddComponent<CoroutineTestRunner>();

        CoroutineStartCounter = 0;
        CoroutineStopCounter = 0;
    }

    public static EventsMono_Mock NewSubstitute()
    {
        var substitute = new EventsMono_Mock();
        substitute.PlayerEvents = Substitute.For<IPlayerEvents>();
        substitute.EnemyEvents = Substitute.For<IEnemyEvents>();
        substitute.SceneEvents = Substitute.For<ISceneEvents>();
        return substitute;
    }

    public void StartCoroutine(IEnumerator coroutine)
    {
        CoroutineTestRunner.StartCoroutine(coroutine);
        CoroutineStartCounter++;
    }

    public void StopCoroutine(IEnumerator coroutine)
    {
        CoroutineTestRunner.StopCoroutine(coroutine);
        CoroutineStopCounter++;
    }
}

public class CoroutineTestRunner : MonoBehaviour
{
    public new void StartCoroutine(IEnumerator coroutine)
    {
        base.StartCoroutine(coroutine);
    }

    public new void StopCoroutine(IEnumerator coroutine)
    {
        base.StopCoroutine(coroutine);
    }
}