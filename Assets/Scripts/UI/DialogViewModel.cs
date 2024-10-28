using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public interface IDialogViewModel
    {
        void IterateScreens();
        void SetActive(bool active);
    }

    public class DialogViewModel : IDialogViewModel
    {
        private int currentScreen;
        
        private readonly List<GameObject> screens;
        private readonly UICoordinator uiCoordinator;
        private readonly GameObject parentGameObject;

        public DialogViewModel(
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
}