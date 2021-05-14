using UnityEngine;

public class DialogTrigger : MonoBehaviour 
{
    [SerializeField] private DialogMono dialogMono = null;
    [SerializeField] private EventsMono eventsMono = null;
    [SerializeField] private bool disableAfterDisplay = false;
    [SerializeField] private bool triggerUponStart = false;
    
    private Collider triggerCollider;
    
    void Awake() 
    {
        triggerCollider = GetComponent<Collider>();
    }

    void Start()
    {
        if (triggerUponStart)
            TriggerDialog();
    }
    void OnTriggerEnter() 
    {
        if (!triggerUponStart)
            TriggerDialog();
    }

    void TriggerDialog()
    {
        eventsMono.SceneEvents.OpenDialog(dialogMono.DialogVM);

        if (disableAfterDisplay)
            triggerCollider.gameObject.SetActive(false);
    }
}
