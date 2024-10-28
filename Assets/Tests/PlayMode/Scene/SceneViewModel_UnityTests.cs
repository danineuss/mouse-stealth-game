using System;
using System.Collections;
using Enemies;
using NSubstitute;
using NUnit.Framework;
using Player;
using Scenes;
using UI;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SceneViewModel_UnityTests
    {
        private IPlayerViewModel playerViewModel;
        private IPlayerEvents playerEvents;
        private IEnemyEvents enemyEvents;
        private ISceneEvents sceneEvents;
        private SceneViewModel sceneViewModel;

        private void SetupSceneViewModel()
        {
            playerViewModel = Substitute.For<IPlayerViewModel>();
            playerEvents = Substitute.For<IPlayerEvents>();
            enemyEvents = Substitute.For<IEnemyEvents>();
            sceneEvents = Substitute.For<ISceneEvents>();
            sceneViewModel = new SceneViewModel(
                playerEvents,
                enemyEvents,
                sceneEvents
            );
        }

        [UnityTest]
        public IEnumerator should_stop_and_resume_time_when_toggeling_paused()
        {
            SetupSceneViewModel();

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
            SetupSceneViewModel();
            var dialogVM = Substitute.For<IDialogViewModel>();

            float timeWhenPausing = Time.time;
            sceneEvents.OnDialogOpened += Raise.Event<Action<IDialogViewModel>>(dialogVM);

            yield return null;

            Assert.AreEqual(timeWhenPausing, Time.time);
            Assert.AreEqual(Time.timeScale, 0f);

            sceneEvents.OnDialogClosed += Raise.Event<Action<IDialogViewModel>>(dialogVM);

            yield return null;

            Assert.Greater(Time.time, timeWhenPausing);
            Assert.AreEqual(Time.timeScale, 1f);
        }

        [UnityTest]
        public IEnumerator should_not_unpause_when_dialog_open()
        {
            SetupSceneViewModel();
            var dialogVM = Substitute.For<IDialogViewModel>();

            float timeWhenPausing = Time.time;
            sceneEvents.OnDialogOpened += Raise.Event<Action<IDialogViewModel>>(dialogVM);

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
            SetupSceneViewModel();

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