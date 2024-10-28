using Player;
using UnityEngine;
using Random = System.Random;

namespace Audio
{
    public interface IPanicNoiseEmitter {}

    public class PanicNoiseEmitter: IPanicNoiseEmitter
    {
        private float timeUntilReadyToPlaySound;

        private readonly IPlayerEvents playerEvents;
        private readonly ISoundEmitter playerSoundEmitter;
        private readonly IAudioViewModel audioViewModel;
        private readonly Random random;

        public PanicNoiseEmitter(IPlayerEvents playerEvents, ISoundEmitter playerSoundEmitter, IAudioViewModel audioViewModel)
        {
            this.playerEvents = playerEvents;
            this.playerSoundEmitter = playerSoundEmitter;
            this.audioViewModel = audioViewModel;

            random = new Random();
            timeUntilReadyToPlaySound = Time.time;

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            playerEvents.OnPanicLevelChanged += OnPanicLevelChanged;
        }

        private void OnPanicLevelChanged(float panicLevel)
        {
            var currentTime = Time.time;
            if (panicLevel < 0.2f || currentTime < timeUntilReadyToPlaySound)
                return;
        
            var sound = SelectSound(panicLevel);
            playerSoundEmitter.PlaySound(sound);
            timeUntilReadyToPlaySound = currentTime + sound.Clip.length;
        }

        private Sound SelectSound(float panicLevel)
        {        
            if (panicLevel < 0.7f)
            {
                var scaredSounds = 
                    new PanicSound[] { PanicSound.ScaredOne, PanicSound.ScaredTwo, PanicSound.ScaredThree };
                var randomSound = scaredSounds[random.Next(scaredSounds.Length)];

                return audioViewModel.SoundWithName(randomSound.Name);
            }
        
            return audioViewModel.SoundWithName(PanicSound.Panicking.Name);
        }
    }
}