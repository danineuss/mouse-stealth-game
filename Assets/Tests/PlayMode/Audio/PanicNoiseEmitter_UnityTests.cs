using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using NSubstitute;
using NUnit.Framework;
using Player;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PanicNoiseEmitter_UnityTests
    {
        private PlayerEvents playerEvents = new PlayerEvents();

        private AudioClip audioClip = AudioClip.Create("mockClip", 1000, 1, 1000, false);
        private Sound panickedSound;
        private IAudioVM SetupAudioVMSubstitute()
        {
            var audioVM = Substitute.For<IAudioVM>();
            panickedSound = 
                new Sound() { Name = "Panicked", Clip = audioClip, Volume = 1f, Loop = false };
            audioVM.SoundWithName(PanicSound.Panicking.Name).Returns(panickedSound);
            return audioVM;
        }   

        [UnityTest]
        public IEnumerator should_not_send_be_able_to_play_sound_clip_twice_until_after_clip_duration()
        {
            // ARRANGE
            var playerSoundEmitter = Substitute.For<ISoundEmitter>();
            var audioVM = SetupAudioVMSubstitute();
            var panicNoiseEmitter = new PanicNoiseEmitter(playerEvents, playerSoundEmitter, audioVM);

            // ACT        
            playerEvents.ChangePanicLevel(0.8f);
            playerEvents.ChangePanicLevel(0.8f);

            yield return null;

            // ASSERT
            playerSoundEmitter.Received(1).PlaySound(panickedSound);
            audioVM.Received(1).SoundWithName(PanicSound.Panicking.Name);

            // ACT
            yield return new WaitForSeconds(1f);

            playerEvents.ChangePanicLevel(0.8f);

            // ASSERT
            playerSoundEmitter.Received(2).PlaySound(panickedSound);
            audioVM.Received(2).SoundWithName(PanicSound.Panicking.Name);        
        }
    }
}