using System;
using Audio;
using Enemies.Detection;
using Player;
using UnityEngine;

namespace Enemies
{
    public interface IEnemyViewModel: IIdentifiable
    {
        void PlaySound(Sound sound);
    }

    public class EnemyViewModel : IEnemyViewModel
    {
        public Guid ID { get; }

        private readonly IPlayerDetector playerDetector;
        private readonly AudioViewModel audioViewModel;
        private readonly IEnemyIO enemyIO;
        private readonly SoundEmitter soundEmitter;
        private readonly IPlayerEvents playerEvents;
        private readonly IEnemyEvents enemyEvents;

        public void PlaySound(Sound sound)
        {
            soundEmitter.PlaySound(sound);
        }

        public EnemyViewModel(
            IPlayerDetector playerDetector,
            AudioViewModel audioViewModel,
            IEnemyIO enemyIO,
            SoundEmitter soundEmitter,
            IPlayerEvents playerEvents,
            IEnemyEvents enemyEvents)
        {
            this.playerDetector = playerDetector;
            this.audioViewModel = audioViewModel;
            this.enemyIO = enemyIO;
            this.soundEmitter = soundEmitter;
            this.playerEvents = playerEvents;
            this.enemyEvents = enemyEvents;
        
            ID = Guid.NewGuid();
            this.enemyIO.SetEnemyID(ID);

            InitializeEvents();
        }

        void InitializeEvents()
        {
            enemyEvents.OnDetectorStateChanged += OnDetectorStateChanged;

            playerEvents.OnPlayerLocationSent += OnReceivePlayerLocation;
            playerEvents.OnPlayerLocationRemoved += OnRemovePlayerLocation;
            playerEvents.OnAbilityExecuted += OnPlayerAbilityExecuted;
            playerEvents.OnEnemyDistracted += OnEnemyDistracted;
        }

        void OnDetectorStateChanged(Guid detectorID)
        {
            if (detectorID != playerDetector.ID)
                return;

            enemyIO.SetTextColor(playerDetector.DetectorState.EnemyIOTextColor);
            var enemySoundName = playerDetector.DetectorState.EnemySound.Name;
            PlaySound(audioViewModel.SoundWithName(enemySoundName));
        }

        void OnReceivePlayerLocation(
            Guid enemyID, bool shouldDisplayText, Transform playerTransform = null)
        {
            if (enemyID != ID)
                return;

            enemyIO.SetTextVisibleAndFollowing(shouldDisplayText, playerTransform);
        }

        void OnRemovePlayerLocation(Guid enemyID)
        {
            if (enemyID != ID)
                return;

            enemyIO.SetTextVisibleAndFollowing(false);
        }

        void OnPlayerAbilityExecuted(IPlayerAbility ability)
        {
            enemyIO.UpdateCooldownForAbility(ability);
        }

        void OnEnemyDistracted(Guid targetID, float distractionDuration)
        {
            if (targetID != ID)
                return;

            if (!playerDetector.AttemptDistraction(distractionDuration))
                return;
        
            enemyIO.SetTextColor(playerDetector.DetectorState.EnemyIOTextColor);
        }
    }
}