using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using Audio;
using Player;
using UnityEngine;

namespace Tests
{
    public class PanicNoiseEmitter_Tests
    {
        private PlayerEvents playerEvents = new PlayerEvents();

        private AudioClip audioClip = AudioClip.Create("mockClip", 1000, 1, 1000, false);
        private Sound scaredSoundOne;
        private Sound scaredSoundTwo;
        private Sound scaredSoundThree;
        private Sound panickedSound;
        

        private IAudioVM SetupAudioVMSubstitute()
        {
            var audioVM = Substitute.For<IAudioVM>();
            scaredSoundOne = 
                new Sound() { Name = "ScaredOne", Clip = audioClip, Volume = 1f, Loop = false };
            scaredSoundTwo =
                new Sound() { Name = "ScaredTwo", Clip = audioClip, Volume = 1f, Loop = false };
            scaredSoundThree = 
                new Sound() { Name = "ScaredThree", Clip = audioClip, Volume = 1f, Loop = false };
            panickedSound = 
                new Sound() { Name = "Panicked", Clip = audioClip, Volume = 1f, Loop = false };
            audioVM.SoundWithName(PanicSound.ScaredOne.Name).Returns(scaredSoundOne);
            audioVM.SoundWithName(PanicSound.ScaredTwo.Name).Returns(scaredSoundTwo);
            audioVM.SoundWithName(PanicSound.ScaredThree.Name).Returns(scaredSoundThree);
            audioVM.SoundWithName(PanicSound.Panicking.Name).Returns(panickedSound);
            return audioVM;
        }   

        [Test]
        public void should_not_emit_any_sound_below_threshold_panic_level()
        {
            // ARRANGE
            var playerSoundEmitter = Substitute.For<ISoundEmitter>();
            var audioVM = SetupAudioVMSubstitute();
            var panicNoiseEmitter = new PanicNoiseEmitter(playerEvents, playerSoundEmitter, audioVM);

            // ACT
            playerEvents.ChangePanicLevel(0f);

            // ASSERT
            playerSoundEmitter.DidNotReceiveWithAnyArgs().PlaySound(default);
            audioVM.DidNotReceiveWithAnyArgs().SoundWithName(default);
        }

        [Test]
        public void should_emit_a_scared_sound_when_panic_level_between_two_thresholds()
        {
            // ARRANGE
            var playerSoundEmitter = Substitute.For<ISoundEmitter>();
            var audioVM = SetupAudioVMSubstitute();
            var panicNoiseEmitter = new PanicNoiseEmitter(playerEvents, playerSoundEmitter, audioVM);

            // ACT
            playerEvents.ChangePanicLevel(0.3f);

            // ASSERT
            playerSoundEmitter.Received(1).PlaySound(
                Arg.Is<Sound>(
                    x => new Sound[] {scaredSoundOne, scaredSoundTwo, scaredSoundThree}.Contains(x)
                )
            );
            audioVM.Received(1).SoundWithName(
                Arg.Is<string>(
                    x => new string[] {
                        scaredSoundOne.Name, scaredSoundTwo.Name, scaredSoundThree.Name
                    }.Contains(x)
                )
            );
        }

        [Test]
        public void should_emit_panicked_sound_when_panic_level_above_threshold()
        {
            // ARRANGE
            var playerSoundEmitter = Substitute.For<ISoundEmitter>();
            var audioVM = SetupAudioVMSubstitute();
            var panicNoiseEmitter = new PanicNoiseEmitter(playerEvents, playerSoundEmitter, audioVM);

            // ACT
            playerEvents.ChangePanicLevel(0.8f);

            // ASSERT
            playerSoundEmitter.Received(1).PlaySound(panickedSound);
            audioVM.Received(1).SoundWithName(PanicSound.Panicking.Name);
        }
    }
}