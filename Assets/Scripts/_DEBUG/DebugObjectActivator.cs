using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugObjectActivator : MonoBehaviour {
    public GameObject Object;
    public bool DesiredStateAfterTrigger;
    
    void OnTriggerEnter() {
        Object.SetActive(DesiredStateAfterTrigger);
    }
}
