using UnityEngine;

namespace Tests
{
    public class DummyAbility : IPlayerAbility
    {
        public KeyCode AssociatedKey => keyCode;
        public float CoolDown => coolDown;
        private KeyCode keyCode;
        private float coolDown;

        public DummyAbility(KeyCode keyCode, float coolDown)
        {
            this.keyCode = keyCode;
            this.coolDown = coolDown;
        }
        public void SetTarget(IEnemyVM target = null) {}

        public void Execute(IEnemyVM enemyVM = null)
        {
            return;
        }
    }
}