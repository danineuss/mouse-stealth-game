using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogVM : MonoBehaviour {
    
    [SerializeField] private List<GameObject> screens;
    
    private UICoordinator uiCoordinator;
    private int currentScreen;
    
    void Awake() {
        uiCoordinator = GetComponentInParent<UICoordinator>();
    }

    void Start() {
        InitializeScreens();
    }

    void InitializeScreens() {
        foreach (var screen in screens) {
            screen.SetActive(false);
        }
        screens[0].SetActive(true);
        currentScreen = 0;    
    }

    public void IterateScreens() {
        if (currentScreen == screens.Count - 1) {
            screens[currentScreen].SetActive(false);
            uiCoordinator.CloseDialog(this);
            return;
        }

        screens[currentScreen].SetActive(false);
        currentScreen++;
        screens[currentScreen].SetActive(true);
    }
}
