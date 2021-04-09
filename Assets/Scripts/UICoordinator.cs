using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UICoordinator : MonoBehaviour
{
    private GameObject failedScreen;

    void Start() {
        failedScreen = GetComponentsInChildren<Transform>()
                        .Where(x => x.CompareTag("FailedScreen"))
                        .First()
                        .gameObject;
        failedScreen.SetActive(false);
    }

    public void HideGamePaused() {
        
    }

    public void ShowGamePaused() {

    }

    public void ShowGameFailed() {
        failedScreen.SetActive(true);
    }
}
