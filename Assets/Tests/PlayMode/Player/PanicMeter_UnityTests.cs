using System;
using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Player;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PanicMeter_UnityTests
    {
        private readonly float panicEscalationSpeed = 4f;
        private readonly float panicDeescalationSpeed = 4f;

        [UnityTest]
        public IEnumerator should_not_send_panic_character_event_at_start()
        {
            var playerEvents = Substitute.For<IPlayerEvents>();
            var panicMeter = new PanicMeter(panicEscalationSpeed, panicDeescalationSpeed, playerEvents);
            var playerGameObject = PlayerMono_Mock.Dummy(panicMeter, playerEvents);

            yield return new WaitForSeconds(0.05f);

            playerEvents.DidNotReceiveWithAnyArgs().PanicCharacter();
            playerEvents.ReceivedWithAnyArgs().ChangePanicLevel(default);
            playerEvents.DidNotReceive().ChangePanicLevel(1f);
        }

        [UnityTest]
        public IEnumerator should_send_panic_character_event_after_short_time_without_cover()
        {
            var playerEvents = Substitute.For<IPlayerEvents>();
            var panicMeter = new PanicMeter(panicEscalationSpeed, panicDeescalationSpeed, playerEvents);
            var playerGameObject = PlayerMono_Mock.Dummy(panicMeter, playerEvents);

            yield return new WaitForSeconds(0.3f);

            playerEvents.Received().PanicCharacter();
            playerEvents.ReceivedWithAnyArgs().ChangePanicLevel(default);
            playerEvents.Received().ChangePanicLevel(1f);
        }
    }
}