using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyIO : MonoBehaviour
{
    private EnemyVM enemyVM;
    private GameObject textDisplay;
    private Outline enemyOutline;
    private Transform playerFollowTransform = null;
    private bool interactible = true;

    public void SetDisplayVisibility(bool visible) {
        if (!interactible) {
            return;
        }

        textDisplay.SetActive(visible);
        enemyOutline.OutlineWidth = visible ? 10 : 0;
    }

    public void SetTextFollowingPlayer(Transform playerTransform = null) {
        playerFollowTransform = playerTransform;
    }

    public void SetInteractible(DetectorState detectorState) {
        var interactible = (detectorState == DetectorState.Idle) ? true : false;
        if (!interactible) {
            SetDisplayVisibility(false); 
        }
        this.interactible = interactible;  
    }

    void Start() {
        enemyVM = GetComponentInParent<EnemyVM>();
        textDisplay = GetComponentsInChildren<Transform>().Where(x => x.CompareTag("InteractiveUI"))
                        .First()
                        .gameObject;
        enemyOutline = enemyVM.GetComponentsInChildren<Transform>().Where(x => x.CompareTag("Model"))
                        .First()
                        .GetComponent<Outline>();

        SetDisplayVisibility(false);
    }

    void Update() {
        UpdateTextOrientation();
    }

    void OnMouseEnter() {
        enemyVM.EnemyEvents.CursorEnterEnemy(enemyVM);
    }

    void OnMouseExit() {
        enemyVM.EnemyEvents.CursorExitEnemy();
    }

    void UpdateTextOrientation() {
        if (playerFollowTransform == null) {
            return;
        }

        transform.LookAt(playerFollowTransform);
    }
}
