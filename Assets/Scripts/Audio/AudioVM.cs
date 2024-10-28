using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class PanicSound
    {
        public string Name { get; private set; }

        private PanicSound(string name) { Name = name; }

        public static PanicSound ScaredOne => new PanicSound("ScaredOne");
        public static PanicSound ScaredTwo => new PanicSound("ScaredTwo");
        public static PanicSound ScaredThree => new PanicSound("ScaredThree");
        public static PanicSound Panicking => new PanicSound("Panicking");
    }

    public class EnemySound
    {
        public string Name { get; private set; }

        private EnemySound(string name) { Name = name; }

        public static EnemySound Idle => new EnemySound("Idle");
        public static EnemySound Searching => new EnemySound("Searching");
        public static EnemySound Distracted => new EnemySound("Distracted");
        public static EnemySound Alarmed => new EnemySound("Alarmed");
    }

    public interface IAudioVM
    {
        Sound SoundWithName(string name);
    }

    public class AudioVM : MonoBehaviour, IAudioVM
    {
        [SerializeField] private SoundEmitter musicSoundEmitter = null;
        [SerializeField] private List<Sound> Sounds = null;


        public Sound SoundWithName(string name)
        {
            return Sounds.Find(s => s.Name == name);
        }

        void Start()
        {
            PlayThemeMusic();
        }

        void PlayThemeMusic()
        {
            Sound theme = SoundWithName("Theme");
            musicSoundEmitter.PlaySound(theme);
        }
    }
}