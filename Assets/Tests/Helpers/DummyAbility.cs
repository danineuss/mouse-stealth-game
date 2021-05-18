using System;
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
    }
}