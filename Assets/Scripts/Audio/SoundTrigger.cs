using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    [SerializeField] private AudioVM audioVM = null;
    [SerializeField] private string nameOfSound = "";
    private SoundEmitter soundEmitter;

    void Awake() {
        soundEmitter = GetComponentInChildren<SoundEmitter>();
    }

    void OnTriggerEnter() {
        
        Sound sound = audioVM.SoundWithName(nameOfSound);
        soundEmitter.PlaySound(sound);
    }
}
