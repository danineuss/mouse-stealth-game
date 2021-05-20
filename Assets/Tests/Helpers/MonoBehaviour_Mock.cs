using UnityEngine;

public class MonoBehaviour_Mock: MonoBehaviour
{
    public IUpdatable Updatable;
    public ILateUpdatable LateUpdatable;
    public ITriggerEnterable TriggerEnterable;

    void Update()
    {
        if (Updatable == null)
            return;
        
        Updatable.Update();
    }

    void LateUpdate()
    {
        if (LateUpdatable == null)
            return;

        LateUpdatable.LateUpdate();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (TriggerEnterable == null)
            return;
        
        TriggerEnterable.OnTriggerEnter(collider);
    }
}