using System.Collections.Generic;
using UnityEngine;

public class DialogMono: MonoBehaviour 
{
    [SerializeField] private UICoordinator uiCoordinator;
    [SerializeField] private List<GameObject> screens;
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
