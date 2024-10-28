using System;
using UnityEngine;

namespace Player
{
    public interface IPlayerEvents
    {
        event Action<IPlayerAbility> OnAbilityExecuted;
        event Action<IPlayerAbility> OnAbilityLearned;
        event Action<Guid, float> OnEnemyDistracted;
        event Action OnPauseButtonPressed;
        event Action<bool> OnCharacterInCoverChanged;
        event Action OnCharacterPanicked;
        event Action<float> OnPanicLevelChanged;
        event Action<Guid> OnPlayerLocationRemoved;
        event Action<Guid, bool, Transform> OnPlayerLocationSent;

        void ChangeCharacterInCover(bool newValue);
        void ChangePanicLevel(float newValue);
        void DistractEnemy(Guid enemyID, float distractionDuration);
        void ExecuteAbility(IPlayerAbility ability);
        void LearnAbility(IPlayerAbility ability);
        void PanicCharacter();
        void PressPauseButton();
        void RemovePlayerLocation(Guid enemyID);
        void SendPlayerLocation(Guid enemyID, bool shouldDisplayText, Transform playerTransform);
    }

    public class PlayerEvents : IPlayerEvents
    {
        public event Action<IPlayerAbility> OnAbilityExecuted;
        public event Action<IPlayerAbility> OnAbilityLearned;
        public event Action<bool> OnCharacterInCoverChanged;
        public event Action OnCharacterPanicked;
        public event Action<Guid, float> OnEnemyDistracted;
        public event Action OnPauseButtonPressed;
        public event Action<float> OnPanicLevelChanged;
        public event Action<Guid> OnPlayerLocationRemoved;
        public event Action<Guid, bool, Transform> OnPlayerLocationSent;

        public void ChangeCharacterInCover(bool newValue)
        {
            if (OnCharacterInCoverChanged == null)
                return;
        
            OnCharacterInCoverChanged(newValue);
        }

        public void ChangePanicLevel(float newValue)
        {
            if (OnPanicLevelChanged == null)
                return;

            OnPanicLevelChanged(newValue);
        }

        public void DistractEnemy(Guid enemyID, float distractionDuration)
        {
            if (OnEnemyDistracted == null)
                return;
        
            OnEnemyDistracted(enemyID, distractionDuration);
        }

        public void ExecuteAbility(IPlayerAbility ability)
        {
            if (OnAbilityExecuted == null)
                return;

            OnAbilityExecuted(ability);
        }

        public void LearnAbility(IPlayerAbility ability)
        {
            if (OnAbilityLearned == null)
                return;

            OnAbilityLearned(ability);
        }

        public void PanicCharacter()
        {
            if (OnCharacterPanicked == null)
                return;
        
            OnCharacterPanicked();
        }

        public void PressPauseButton()
        {
            if (OnPauseButtonPressed == null)
                return;
        
            OnPauseButtonPressed();
        }

        public void RemovePlayerLocation(Guid enemyID)
        {
            if (OnPlayerLocationRemoved == null)
                return;

            OnPlayerLocationRemoved(enemyID);
        }

        public void SendPlayerLocation(
            Guid enemyID, bool shouldDisplayText, Transform playerTransform)
        {
            if (OnPlayerLocationSent == null)
                return;

            OnPlayerLocationSent(enemyID, shouldDisplayText, playerTransform);
        }
    }
}