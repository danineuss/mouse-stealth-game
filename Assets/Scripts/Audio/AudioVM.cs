using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
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