using System;
using UnityEngine;

namespace Player
{
    public class PlayerAbilityDistract : IPlayerAbility
    {
        public KeyCode AssociatedKey => KeyCode.F;
        public float CoolDown => 10f;

        private readonly float distractionDuration;
        private Guid targetID;

        public PlayerAbilityDistract(float distractionDuration)
        {
            this.distractionDuration = distractionDuration;
        }

        public void SetTarget(Guid targetID)
        {
            this.targetID = targetID;
        }

        public void Execute(IPlayerEvents playerEvents) 
        {
            playerEvents.DistractEnemy(targetID, distractionDuration);
        }

        public bool Equals(IPlayerAbility other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (this is null || other is null)
                return false;

            return AssociatedKey == other.AssociatedKey && 
                   CoolDown == other.CoolDown;
        }
    }
}
