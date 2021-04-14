using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyIO : MonoBehaviour
{
    private EnemyVM enemyVM;
    private GameObject textDisplay;
    private Outline enemyOutline;

    void Start() {
        enemyVM = GetComponentInParent<EnemyVM>();
        textDisplay = GetComponentsInChildren<Transform>().Where(x => x.CompareTag("InteractiveUI"))
                        .First()
                        .gameObject;
        textDisplay.SetActive(false);
        enemyOutline = enemyVM.GetComponentsInChildren<Transform>().Where(x => x.CompareTag("Model"))
                        .First()
                        .GetComponent<Outline>();
                        
        ToggleDisplayVisibility(false);
    }

    void OnMouseEnter() {
        enemyVM.EnemyEvents.CursorEnterEnemy(enemyVM);
    }

    void OnMouseExit() {
        enemyVM.EnemyEvents.CursorExitEnemy();
    }

    public void ToggleDisplayVisibility(bool visible) {
        textDisplay.SetActive(visible);

        if (visible) {
            enemyOutline.OutlineWidth = 10;
        } else {
            enemyOutline.OutlineWidth = 0;
        }
    }
}
