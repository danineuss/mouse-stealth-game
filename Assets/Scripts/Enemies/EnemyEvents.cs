using System;

public class EnemyEvents : IEnemyEvents
{
    public event Action<EnemyVM> OnCursorEnterEnemy;
    public event Action OnCurserExitEnemy;
    public event Action<PlayerDetector> OnDetectorStateChanged;

    public void CursorEnterEnemy(EnemyVM enemyVM = null)
    {
        if (OnCursorEnterEnemy == null)
            return;

        OnCursorEnterEnemy(enemyVM);
    }

    public void CursorExitEnemy()
    {
        if (OnCurserExitEnemy == null)
            return;

        OnCurserExitEnemy();
    }

    public void ChangeDetectorState(PlayerDetector playerDetector)
    {
        if (OnDetectorStateChanged == null)
            return;

        OnDetectorStateChanged(playerDetector);
    }
}
