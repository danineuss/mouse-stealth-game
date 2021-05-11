using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
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
        public void should_show_abilities_after_having_learned_them() 
        {
            var playerEvents = Substitute.For<IPlayerEvents>();
            var playerAbilities = new PlayerAbilities(playerEvents, new Dictionary<KeyCode, IPlayerAbility>());

            playerAbilities.LearnAbility(firstAbility);

            var relevantAbilities = playerAbilities.RelevantAbilities;
            var relevantKeyPresses = playerAbilities.RelevantKeyPresses;

            Assert.AreEqual(new List<IPlayerAbility>() { firstAbility }, relevantAbilities);
            Assert.AreEqual(new List<KeyCode>() { firstAbility.AssociatedKey }, relevantKeyPresses);
        }

        [Test]
        public void should_not_learn_ability_twice()
        {
             var playerEvents = Substitute.For<IPlayerEvents>();
            var playerAbilities = new PlayerAbilities(playerEvents, new Dictionary<KeyCode, IPlayerAbility>());

            playerAbilities.LearnAbility(firstAbility);
            playerAbilities.LearnAbility(firstAbility);

            var relevantAbilities = playerAbilities.RelevantAbilities;
            var relevantKeyPresses = playerAbilities.RelevantKeyPresses;

            Assert.AreEqual(new List<IPlayerAbility>() { firstAbility }, relevantAbilities);
            Assert.AreEqual(new List<KeyCode>() { firstAbility.AssociatedKey }, relevantKeyPresses);
        }
        
        [Test]
        public void should_fire_event_on_ability_executed()
        {
            var playerEvents = Substitute.For<IPlayerEvents>();
            var abilityDictionary = new Dictionary<KeyCode, IPlayerAbility> {
                { KeyCode.A, firstAbility }
            };
            var playerAbilities = new PlayerAbilities(playerEvents, abilityDictionary);

            playerAbilities.ExecuteAbility(firstAbility);

            playerEvents.Received().AbilityExecuted(firstAbility);
        }

        [Test]
        public void should_not_fire_event_if_no_ability_learned()
        {
            var playerEvents = Substitute.For<IPlayerEvents>();
            var playerAbilities = new PlayerAbilities(playerEvents, new Dictionary<KeyCode, IPlayerAbility>());

            playerAbilities.ExecuteAbility(firstAbility);

            playerEvents.DidNotReceive().AbilityExecuted(firstAbility);
        }
    }
}
