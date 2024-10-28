using System.Collections.Generic;
using Infrastructure;
using UnityEngine;

public class MonoBehaviour_Mock: MonoBehaviour
{
    public List<IUpdatable> Updatables = new List<IUpdatable>();
    public List<ILateUpdatable> LateUpdatables = new List<ILateUpdatable>();
    public List<ITriggerEnterable> TriggerEnterables = new List<ITriggerEnterable>();

    void Update()
    {
        if (Updatables.Count == 0)
            return;
        
        Updatables.ForEach(x => x.Update());
    }

    void LateUpdate()
    {
        if (LateUpdatables.Count == 0)
            return;

        LateUpdatables.ForEach(x => x.LateUpdate());
    }

    void OnTriggerEnter(Collider collider)
    {
        if (TriggerEnterables.Count == 0)
            return;
        
        TriggerEnterables.ForEach(x => x.OnTriggerEnter(collider));
    }
}