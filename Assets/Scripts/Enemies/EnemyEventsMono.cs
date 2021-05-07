using UnityEngine;

public class EnemyEventsMono : MonoBehaviour
{
    public EnemyEvents EnemyEvents => enemyEvents;
    private EnemyEvents enemyEvents;

    void Awake()
    {
        enemyEvents = new EnemyEvents();
    }
}
