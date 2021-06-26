using System;
using UnityEngine;
using Random = System.Random;

public interface IPanicNoiseEmitter {}

public class PanicNoiseEmitter: IPanicNoiseEmitter
{
    private float timeUntilReadyToPlaySound;

    private readonly IPlayerEvents playerEvents;
    private readonly ISoundEmitter playerSoundEmitter;
    private readonly IAudioVM audioVM;
    private readonly Random random;

    public PanicNoiseEmitter(IPlayerEvents playerEvents, ISoundEmitter playerSoundEmitter, IAudioVM audioVM)
    {
        this.playerEvents = playerEvents;
        this.playerSoundEmitter = playerSoundEmitter;
        this.audioVM = audioVM;

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

            return audioVM.SoundWithName(randomSound.Name);
        }
        
        return audioVM.SoundWithName(PanicSound.Panicking.Name);
    }
}