using NSubstitute;
using NUnit.Framework;
using System.Linq;
using Audio;
using Player;
using UnityEngine;

namespace Tests
{
    public class PanicNoiseEmitterTests
    {
        private Sound scaredSoundOne;
        private Sound scaredSoundTwo;
        private Sound scaredSoundThree;
        private Sound panickedSound;
        
        private readonly PlayerEvents playerEvents = new PlayerEvents();
        private readonly AudioClip audioClip = AudioClip.Create(
            "mockClip", 1000, 1, 1000, false);

        private IAudioViewModel SetupAudioViewModelSubstitute()
        {
            var audioViewModelSubstitute = Substitute.For<IAudioViewModel>();
            scaredSoundOne = 
                new Sound() { Name = "ScaredOne", Clip = audioClip, Volume = 1f, Loop = false };
            scaredSoundTwo =
                new Sound() { Name = "ScaredTwo", Clip = audioClip, Volume = 1f, Loop = false };
            scaredSoundThree = 
                new Sound() { Name = "ScaredThree", Clip = audioClip, Volume = 1f, Loop = false };
            panickedSound = 
                new Sound() { Name = "Panicked", Clip = audioClip, Volume = 1f, Loop = false };
            audioViewModelSubstitute.SoundWithName(PanicSound.ScaredOne.Name).Returns(scaredSoundOne);
            audioViewModelSubstitute.SoundWithName(PanicSound.ScaredTwo.Name).Returns(scaredSoundTwo);
            audioViewModelSubstitute.SoundWithName(PanicSound.ScaredThree.Name).Returns(scaredSoundThree);
            audioViewModelSubstitute.SoundWithName(PanicSound.Panicking.Name).Returns(panickedSound);
            return audioViewModelSubstitute;
        }   

        [Test]
        public void should_not_emit_any_sound_below_threshold_panic_level()
        {
            // ARRANGE
            var playerSoundEmitter = Substitute.For<ISoundEmitter>();
            var audioViewModel = SetupAudioViewModelSubstitute();
            _ = new PanicNoiseEmitter(playerEvents, playerSoundEmitter, audioViewModel);

            // ACT
            playerEvents.ChangePanicLevel(0f);

            // ASSERT
            playerSoundEmitter.DidNotReceiveWithAnyArgs().PlaySound(default);
            audioViewModel.DidNotReceiveWithAnyArgs().SoundWithName(default);
        }

        [Test]
        public void should_emit_a_scared_sound_when_panic_level_between_two_thresholds()
        {
            // ARRANGE
            var playerSoundEmitter = Substitute.For<ISoundEmitter>();
            var audioViewModel = SetupAudioViewModelSubstitute();
            _ = new PanicNoiseEmitter(playerEvents, playerSoundEmitter, audioViewModel);

            // ACT
            playerEvents.ChangePanicLevel(0.3f);

            // ASSERT
            playerSoundEmitter.Received(1).PlaySound(
                Arg.Is<Sound>(
                    x => new[] {scaredSoundOne, scaredSoundTwo, scaredSoundThree}.Contains(x)
                )
            );
            audioViewModel.Received(1).SoundWithName(
                Arg.Is<string>(
                    x => new[] {
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
            var audioViewModel = SetupAudioViewModelSubstitute();
            _ = new PanicNoiseEmitter(playerEvents, playerSoundEmitter, audioViewModel);

            // ACT
            playerEvents.ChangePanicLevel(0.8f);

            // ASSERT
            playerSoundEmitter.Received(1).PlaySound(panickedSound);
            audioViewModel.Received(1).SoundWithName(PanicSound.Panicking.Name);
        }
    }
}