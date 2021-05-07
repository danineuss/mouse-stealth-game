using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
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

        public bool Execute(EnemyVM enemyVM = null)
        {
            return true;
        }
    }

    public class PlayerAbilities_Tests
    {
        private IPlayerAbility firstAbility = new DummyAbility(KeyCode.A, 1f);
        private IPlayerAbility secondAbility = new DummyAbility(KeyCode.F, 2f);
        
        [Test]
        public void should_be_empty_if_initialized_emptily() 
        {
            var playerEvents = Substitute.For<IPlayerEvents>();
            var playerAbilities = new PlayerAbilities(playerEvents, new Dictionary<KeyCode, IPlayerAbility>());

            var relevantAbilities = playerAbilities.RelevantAbilities;
            var relevantKeyPresses = playerAbilities.RelevantKeyPresses;

            Assert.AreEqual(0, relevantAbilities.Count);
            Assert.AreEqual(0, relevantKeyPresses.Count);
        }

        [Test]
        public void should_have_correct_content_with_two_abilities() 
        {
            var playerEvents = Substitute.For<IPlayerEvents>();
            var abilityDictionary = new Dictionary<KeyCode, IPlayerAbility> {
                { KeyCode.A, firstAbility },
                { KeyCode.F, secondAbility }
            };
            var playerAbilities = new PlayerAbilities(playerEvents, abilityDictionary);

            var relevantAbilities = playerAbilities.RelevantAbilities;
            var relevantKeyPresses = playerAbilities.RelevantKeyPresses;

            Assert.AreEqual(new List<IPlayerAbility>(){ firstAbility, secondAbility }, relevantAbilities);
            Assert.AreEqual(new List<KeyCode>() { KeyCode.A, KeyCode.F }, relevantKeyPresses);
        }

        [Test]
        public void should_have_show_abilities_after_having_learned_them() 
        {
            var playerEvents = Substitute.For<IPlayerEvents>();
            var playerAbilities = new PlayerAbilities(playerEvents, new Dictionary<KeyCode, IPlayerAbility>());

            playerAbilities.LearnAbility(firstAbility);

            var relevantAbilities = playerAbilities.RelevantAbilities;
            var relevantKeyPresses = playerAbilities.RelevantKeyPresses;

            Assert.AreEqual(new List<IPlayerAbility>() { firstAbility }, relevantAbilities);
            Assert.AreEqual(new List<KeyCode>() { firstAbility.AssociatedKey }, relevantKeyPresses);
        }
    }
}
