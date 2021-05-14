using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class AudioVM : MonoBehaviour {
    [SerializeField] private PlayerMono playerMono = null;
    [SerializeField] private List<Sound> Sounds = null;
    private SoundEmitter playerSoundEmitter;

    public void PlaySoundAtEnemy(IEnemyVM enemyVM, DetectorStateEnum detectorState) {
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

    Sound SoundMapping(DetectorStateEnum detectorState) {
        switch(detectorState) {
            case DetectorStateEnum.Idle:
                return Sounds.Find(s => s.Name == "Idle");
            case DetectorStateEnum.Searching:
                return Sounds.Find(s => s.Name == "Searching");
            case DetectorStateEnum.Alarmed:
                return Sounds.Find(s => s.Name == "Alarmed");
            case DetectorStateEnum.Distracted:
                return Sounds.Find(s => s.Name == "Distracted");
            default:
                throw new InvalidOperationException("Switch case not exhaustive: " + detectorState);
        }
    }
}
