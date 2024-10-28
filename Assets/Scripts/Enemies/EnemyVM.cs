using System;
using Audio;
using Enemies.Detection;
using Player;
using UnityEngine;

namespace Enemies
{
    public interface IEnemyVM: IIdentifiable
    {
        void PlaySound(Sound sound);
    }

    public class EnemyVM : IEnemyVM
    {
        public Guid ID { get; private set; }

        private IPlayerDetector playerDetector;
        private AudioVM audioVM;
        private IEnemyIO enemyIO;
        private SoundEmitter soundEmitter;
        private IPlayerEvents playerEvents;
        private IEnemyEvents enemyEvents;

        public void PlaySound(Sound sound)
        {
            soundEmitter.PlaySound(sound);
        }

        public EnemyVM(
            IPlayerDetector playerDetector,
            AudioVM audioVM,
            IEnemyIO enemyIO,
            SoundEmitter soundEmitter,
            IPlayerEvents playerEvents,
            IEnemyEvents enemyEvents)
        {
            this.playerDetector = playerDetector;
            this.audioVM = audioVM;
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
            PlaySound(audioVM.SoundWithName(enemySoundName));
        }

        void OnReceivePlayerLocation(
            Guid enemyID, bool shouldDisplayText, Transform playerTransform = null)
        {
            if (enemyID != this.ID)
                return;

            enemyIO.SetTextVisibleAndFollowing(shouldDisplayText, playerTransform);
        }

        void OnRemovePlayerLocation(Guid enemyID)
        {
            if (enemyID != this.ID)
                return;

            enemyIO.SetTextVisibleAndFollowing(false);
        }

        void OnPlayerAbilityExecuted(IPlayerAbility ability)
        {
            enemyIO.UpdateCooldownForAbility(ability);
        }

        void OnEnemyDistracted(Guid targetID, float distractionDuration)
        {
            if (targetID != this.ID)
                return;

            if (!playerDetector.AttemptDistraction(distractionDuration))
                return;
        
            enemyIO.SetTextColor(playerDetector.DetectorState.EnemyIOTextColor);
        }
    }
}