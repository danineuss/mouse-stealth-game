using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class SoundEmitter : MonoBehaviour {
    private AudioSource audioSource;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(Sound sound) {
        audioSource.clip = sound.Clip;
        audioSource.volume = sound.Volume;
        audioSource.loop = sound.Loop;

        audioSource.Play();
    }
}
