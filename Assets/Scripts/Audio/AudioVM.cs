using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class EnemySound
{
    public string Name { get; private set; }

    private EnemySound(string name) { Name = name; }

    public static EnemySound Idle => new EnemySound("Idle");
    public static EnemySound Searching => new EnemySound("Searching");
    public static EnemySound Distracted => new EnemySound("Distracted");
    public static EnemySound Alarmed => new EnemySound("Alarmed");
}

public class AudioVM : MonoBehaviour {
    [SerializeField] private PlayerMono playerMono = null;
    [SerializeField] private List<Sound> Sounds = null;

    private SoundEmitter playerSoundEmitter;

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
        Sound theme = SoundWithName("Theme");
        playerSoundEmitter.PlaySound(theme);
    }
}
