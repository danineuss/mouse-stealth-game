using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyEvents : MonoBehaviour
{
    public event Action OnCursorEnterEnemy;
    public event Action OnCurserExitEnemy;

    public void CursorEnterEnemy(EnemyVM enemy = null) {
        if (OnCursorEnterEnemy == null) {
            return;
        }
        OnCursorEnterEnemy();
    }
    
    public void CursorExitEnemy(EnemyVM enemy = null) {
        if (OnCurserExitEnemy == null) {
            return;
        }
        OnCurserExitEnemy();
    }
}
