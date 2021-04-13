using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyIO : MonoBehaviour
{
    private EnemyVM enemyVM;
    private GameObject textDisplay;

    void Start() {
        enemyVM = GetComponentInParent<EnemyVM>();
        textDisplay = GetComponentsInChildren<Transform>().Where(x => x.CompareTag("InteractiveUI"))
                        .First()
                        .gameObject;
        textDisplay.SetActive(false);
    }

    void OnMouseEnter() {
        enemyVM.EnemyEvents.CursorEnterEnemy(enemyVM);
    }

    void OnMouseExit() {
        enemyVM.EnemyEvents.CursorExitEnemy();
    }

    public void ToggleDisplayVisibility(bool visible) {
        textDisplay.SetActive(visible);
    }
}
