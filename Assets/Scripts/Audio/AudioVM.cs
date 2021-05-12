using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class AudioVM : MonoBehaviour {
    [SerializeField] private PlayerMono playerMono;
    [SerializeField] private List<Sound> Sounds;
    private SoundEmitter playerSoundEmitter;

    public void PlaySoundAtEnemy(IEnemyVM enemyVM, DetectorState detectorState) {
        Sound sound = SoundMapping(detectorState);
        enemyVM.PlaySound(sound);
    }

    public Sound SoundWithName(string name) {
        return Sounds.Find(s => s.Name == name);
    }

    void Awake() {
        playerSoundEmitter = playerMono.GetComponentInChildren<SoundEmitter>();
    }

    void Start() {
        PlayThemeMusic();
    }

    void PlayThemeMusic() {
        Sound theme = Sounds.Find(s => s.Name == "Theme");
        playerSoundEmitter.PlaySound(theme);
    }

    Sound SoundMapping(DetectorState detectorState) {
        switch(detectorState) {
            case DetectorState.Idle:
                return Sounds.Find(s => s.Name == "Idle");
            case DetectorState.Searching:
                return Sounds.Find(s => s.Name == "Searching");
            case DetectorState.Alarmed:
                return Sounds.Find(s => s.Name == "Alarmed");
            case DetectorState.Distracted:
                return Sounds.Find(s => s.Name == "Distracted");
            default:
                throw new InvalidOperationException("Switch case not exhaustive: " + detectorState);
        }
    }
}
