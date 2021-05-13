using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SceneVM_UnityTests
    {
        private IPlayerVM playerVM;
        private IPlayerEvents playerEvents;
        private IEnemyEvents enemyEvents;
        private ISceneEvents sceneEvents;
        private SceneVM sceneVM;

        private void SetupSceneVM()
        {
            playerVM = Substitute.For<IPlayerVM>();
            playerEvents = Substitute.For<IPlayerEvents>();
            enemyEvents = Substitute.For<IEnemyEvents>();
            sceneEvents = Substitute.For<ISceneEvents>();
            sceneVM = new SceneVM(
                playerEvents,
                enemyEvents,
                sceneEvents
            );
        }

        [UnityTest]
        public IEnumerator should_stop_and_resume_time_when_toggeling_paused()
        {
            SetupSceneVM();

            float timeWhenPausing = Time.time;
            playerEvents.OnPauseButtonPressed += Raise.Event<Action>();

            yield return null;

            Assert.AreEqual(timeWhenPausing, Time.time);
            Assert.AreEqual(Time.timeScale, 0f);
            sceneEvents.Received(1).PauseGame(default);

            playerEvents.OnPauseButtonPressed += Raise.Event<Action>();

            yield return null;

            Assert.Greater(Time.time, timeWhenPausing);
            Assert.AreEqual(Time.timeScale, 1f);
            sceneEvents.Received(2).PauseGame(default);
        }

        [UnityTest]
        public IEnumerator should_stop_and_resume_time_when_toggeling_dialog()
        {
            SetupSceneVM();
            var dialogVM = Substitute.For<IDialogVM>();

            float timeWhenPausing = Time.time;
            sceneEvents.OnDialogOpened += Raise.Event<Action<IDialogVM>>(dialogVM);

            yield return null;

            Assert.AreEqual(timeWhenPausing, Time.time);
            Assert.AreEqual(Time.timeScale, 0f);

            sceneEvents.OnDialogClosed += Raise.Event<Action<IDialogVM>>(dialogVM);

            yield return null;

            Assert.Greater(Time.time, timeWhenPausing);
            Assert.AreEqual(Time.timeScale, 1f);
        }

        [UnityTest]
        public IEnumerator should_not_unpause_when_dialog_open()
        {
            SetupSceneVM();
            var dialogVM = Substitute.For<IDialogVM>();

            float timeWhenPausing = Time.time;
            sceneEvents.OnDialogOpened += Raise.Event<Action<IDialogVM>>(dialogVM);

            yield return null;
            
            Assert.AreEqual(Time.time, timeWhenPausing);
            Assert.AreEqual(Time.timeScale, 0f);

            playerEvents.OnPauseButtonPressed += Raise.Event<Action>();

            yield return null;

            Assert.AreEqual(Time.time, timeWhenPausing);
            Assert.AreEqual(Time.timeScale, 0f);            
        }

        [UnityTest]
        public IEnumerator should_not_unpause_when_game_failed()
        {
            SetupSceneVM();

            float timeWhenPausing = Time.time;
            enemyEvents.OnGameFailed += Raise.Event<Action>();

            yield return null;
            
            Assert.AreEqual(Time.time, timeWhenPausing);
            Assert.AreEqual(Time.timeScale, 0f);

            playerEvents.OnPauseButtonPressed += Raise.Event<Action>();

            yield return null;

            Assert.AreEqual(Time.time, timeWhenPausing);
            Assert.AreEqual(Time.timeScale, 0f);            
        }
    }
}