using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class DialogMono: MonoBehaviour 
    {
        [SerializeField] private UICoordinator uiCoordinator;
        [SerializeField] private List<GameObject> screens;
        public IDialogViewModel DialogViewModel => dialogViewModel;
        private IDialogViewModel dialogViewModel;

        void Awake() 
        {
            dialogViewModel = new DialogViewModel(screens, uiCoordinator, gameObject);
        }

        public void IterateScreens()
        {
            dialogViewModel.IterateScreens();
        }
    }
}
