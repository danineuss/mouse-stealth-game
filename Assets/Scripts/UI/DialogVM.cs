using System.Collections.Generic;
using UnityEngine;

public interface IDialogVM
{
    void IterateScreens();
    void SetActive(bool active);
}

public class DialogVM : IDialogVM
{
    private List<GameObject> screens;
    private UICoordinator uiCoordinator;
    private GameObject parentGameObject;
    private int currentScreen;

    public DialogVM(
        List<GameObject> screens, 
        UICoordinator uiCoordinator, 
        GameObject parentGameObject)
    {
        this.screens = screens;
        this.uiCoordinator = uiCoordinator;
        this.parentGameObject = parentGameObject;

        InitializeScreens();
    }

    void InitializeScreens()
    {
        screens.ForEach(screen => screen.SetActive(false));
        screens[0].SetActive(true);
        currentScreen = 0;
    }

    public void IterateScreens()
    {
        if (currentScreen == screens.Count - 1)
        {
            uiCoordinator.SceneEvents.CloseDialog(this);
            return;
        }

        screens[currentScreen].SetActive(false);
        currentScreen++;
        screens[currentScreen].SetActive(true);
    }

    public void SetActive(bool active)
    {
        parentGameObject.SetActive(active);
    }
}
