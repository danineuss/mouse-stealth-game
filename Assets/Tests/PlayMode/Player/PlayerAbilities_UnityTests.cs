using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlayerAbilities_UnityTests
    {
        private IPlayerAbility firstAbility = new DummyAbility(KeyCode.A, 1f);

        [UnityTest]
        public IEnumerator should_fire_ability_a_second_time_only_after_delay()
        {
            var playerEvents = Substitute.For<IPlayerEvents>();
            var abilityDictionary = new Dictionary<KeyCode, IPlayerAbility> {
                { KeyCode.A, firstAbility }
            };
            var playerAbilities = new PlayerAbilities(playerEvents, abilityDictionary);

            playerAbilities.ExecuteAbility(firstAbility);
            playerEvents.Received(1).AbilityExecuted(firstAbility);

            playerAbilities.ExecuteAbility(firstAbility);
            playerEvents.Received(1).AbilityExecuted(firstAbility);

            yield return new WaitForSeconds(firstAbility.CoolDown);

            playerAbilities.ExecuteAbility(firstAbility);
            playerEvents.Received(2).AbilityExecuted(firstAbility);
        }
    }
}