using System;

public interface IEnemyEvents
{
    event Action<EnemyVM> OnCursorEnterEnemy;
    event Action OnCurserExitEnemy;
    event Action<PlayerDetector> OnDetectorStateChanged;

    void ChangeDetectorState(PlayerDetector playerDetector);
    void CursorEnterEnemy(EnemyVM enemyVM = null);
    void CursorExitEnemy();
}
