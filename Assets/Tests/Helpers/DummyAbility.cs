using System;
using Player;
using UnityEngine;

namespace Tests
{
    public class DummyAbility : IPlayerAbility
    {
        public KeyCode AssociatedKey { get; private set; }
        public float CoolDown { get; private set; }

        public DummyAbility(KeyCode keyCode, float coolDown)
        {
            this.AssociatedKey = keyCode;
            this.CoolDown = coolDown;
        }

        public void SetTarget(Guid targetID) {}

        public void Execute(IPlayerEvents playerEvents) {}

        public bool Equals(IPlayerAbility other)
        {
            if (object.ReferenceEquals(this, other))
                return true;

            if (this is null || other is null)
                return false;

            return this.AssociatedKey == other.AssociatedKey && 
                this.CoolDown == other.CoolDown;
        }
    }
}