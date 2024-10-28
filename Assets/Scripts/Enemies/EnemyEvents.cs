using System;

namespace Enemies
{
    public interface IEnemyEvents
    {
        event Action<Guid> OnCursorEnterEnemy;
        event Action OnCurserExitEnemy;
        event Action<Guid> OnDetectorStateChanged;
        event Action OnGameFailed;

        void ChangeDetectorState(Guid detectorID);
        void CursorEnterEnemy(Guid enemyID);
        void CursorExitEnemy();
        void FailGame();
    }

    public class EnemyEvents : IEnemyEvents
    {
        public event Action<Guid> OnCursorEnterEnemy;
        public event Action OnCurserExitEnemy;
        public event Action<Guid> OnDetectorStateChanged;
        public event Action OnGameFailed;

        public void CursorEnterEnemy(Guid enemyID)
        {
            if (OnCursorEnterEnemy == null)
                return;

            OnCursorEnterEnemy(enemyID);
        }

        public void CursorExitEnemy()
        {
            if (OnCurserExitEnemy == null)
                return;

            OnCurserExitEnemy();
        }

        public void ChangeDetectorState(Guid detectorID)
        {
            if (OnDetectorStateChanged == null)
                return;

            OnDetectorStateChanged(detectorID);
        }

        public void FailGame()
        {
            if (OnGameFailed == null)
                return;
        
            OnGameFailed();
        }
    }
}