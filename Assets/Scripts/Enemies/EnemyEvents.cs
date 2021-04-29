using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyEvents : MonoBehaviour {
    public event Action<EnemyVM> OnCursorEnterEnemy;
    public event Action OnCurserExitEnemy;
    public event Action<PlayerDetector> OnDetectorChangedState;

    public void CursorEnterEnemy(EnemyVM enemyVM = null) {
        if (OnCursorEnterEnemy == null)
            return;

        OnCursorEnterEnemy(enemyVM);
    }
    
    public void CursorExitEnemy() {
        if (OnCurserExitEnemy == null)
            return;

        OnCurserExitEnemy();
    }

    public void DetectorChangedState(PlayerDetector playerDetector) {
        if (OnDetectorChangedState == null)
            return;

        OnDetectorChangedState(playerDetector);
    }
}
