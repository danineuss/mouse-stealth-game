using UnityEngine;

namespace Player
{
    public interface IPanicMeter {
        void UpdatePanicLevel();
    }

    public class PanicMeter: IPanicMeter
    {
        private float panicLevel = 0f;
        private bool characterInCover;
        private IPlayerEvents playerEvents;

        private readonly float panicEscalationSpeed;
        private readonly float panicDeescalationSpeed;

        public PanicMeter(float panicEscalationSpeed, float panicDeescalationSpeed, IPlayerEvents playerEvents)
        {
            this.panicEscalationSpeed = panicEscalationSpeed;
            this.panicDeescalationSpeed = panicDeescalationSpeed;
            this.playerEvents = playerEvents;

            InitializeEvents();
        }

        public void UpdatePanicLevel()
        {
            if (characterInCover)
                panicLevel -= panicDeescalationSpeed * Time.deltaTime;
            else
                panicLevel += panicEscalationSpeed * Time.deltaTime;
            panicLevel = Mathf.Clamp(panicLevel, 0f, 1f);

            BroadcastPanicLevel();
        }

        private void BroadcastPanicLevel()
        {
            playerEvents.ChangePanicLevel(panicLevel);
            if (panicLevel == 1f)
                playerEvents.PanicCharacter();
        }

        private void InitializeEvents()
        {
            playerEvents.OnCharacterInCoverChanged += OnPlayerInCoverChanged;
        }

        private void OnPlayerInCoverChanged(bool newValue)
        {
            characterInCover = newValue;
        }
    }
}