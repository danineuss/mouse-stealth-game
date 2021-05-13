using System;
public interface IEnemyEvents
{
    event Action<IEnemyVM> OnCursorEnterEnemy;
    event Action OnCurserExitEnemy;
    event Action<PlayerDetector> OnDetectorStateChanged;
    event Action OnGameFailed;

    void ChangeDetectorState(PlayerDetector playerDetector);
    void CursorEnterEnemy(IEnemyVM enemyVM = null);
    void CursorExitEnemy();
    void FailGame();
}

public class EnemyEvents : IEnemyEvents
{
    public event Action<IEnemyVM> OnCursorEnterEnemy;
    public event Action OnCurserExitEnemy;
    public event Action<PlayerDetector> OnDetectorStateChanged;
    public event Action OnGameFailed;

    public void CursorEnterEnemy(IEnemyVM enemyVM = null)
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

    public void FailGame()
    {
        if (OnGameFailed == null)
            return;
        
        OnGameFailed();
    }
}
