using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class DialogMono: MonoBehaviour 
    {
        [SerializeField] private UICoordinator uiCoordinator = null;
        [SerializeField] private List<GameObject> screens = null;
        public IDialogVM DialogVM => dialogVM;
        private IDialogVM dialogVM;

        void Awake() 
        {
            dialogVM = new DialogVM(screens, uiCoordinator, gameObject);
        }

        public void IterateScreens()
        {
            dialogVM.IterateScreens();
        }
    }
}
