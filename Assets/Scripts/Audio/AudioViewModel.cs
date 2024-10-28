using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public interface IAudioViewModel
    {
        Sound SoundWithName(string soundName);
    }

    public class AudioViewModel : MonoBehaviour, IAudioViewModel
    {
        [SerializeField] private SoundEmitter musicSoundEmitter;
        [SerializeField] private List<Sound> sounds;


        public Sound SoundWithName(string soundName)
        {
            return sounds.Find(s => s.Name == soundName);
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