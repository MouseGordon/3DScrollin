using System;
using System.Collections;
using GameEvent;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlaymodeTests{
    [TestFixture]
    public class CoolDownGameEventIntegrationTests : MonoBehaviour{
        private MonoBehaviour _testGameObject;
        private CoolDownGameEvent _coolDownEvent;
        private bool _startEventFired;
        private bool _endEventFired;

        private Action _startHandler;
        private Action<bool> _endHandler;

        [UnitySetUp]
        public IEnumerator SetUp(){
            _testGameObject = new GameObject("TestObject").AddComponent<TestMonoBehaviour>();
            _coolDownEvent = ScriptableObject.CreateInstance<CoolDownGameEvent>();
            _startEventFired = false;
            _endEventFired = false;

            _startHandler = () => _startEventFired = true;
            _endHandler = (success) => _endEventFired = success;
            // Subscribe to events
            _coolDownEvent.StartCoolDownAction += _startHandler;
            _coolDownEvent.EventAction += _endHandler;

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown(){

            _coolDownEvent.StartCoolDownAction -= _startHandler;
            _coolDownEvent.EventAction -= _endHandler;
            if (_testGameObject != null){
                Destroy(_testGameObject);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator Cooldown_WaitForActualDuration_ShouldComplete(
            [Values(.2f, .5f, 1.0f, 1.5f)] float duration){
            // Arrange
            float startTime = Time.time;
            _coolDownEvent.CoolDownDuration = duration;

            // Act
            _coolDownEvent.StartCoolDown(_testGameObject);

            // Wait for cooldown to complete
            yield return new WaitUntil(() => _endEventFired);
            float elapsedTime = Time.time - startTime;

            // Assert
            Assert.That(_startEventFired, Is.True, "Start event should have fired");
            Assert.That(_endEventFired, Is.True, "End event should have fired");
            Assert.That(elapsedTime, Is.GreaterThanOrEqualTo(duration)
                    .And.LessThan(duration + 0.1f),
                $"Cooldown took {elapsedTime}s, expected about {duration}s");
        }

        [UnityTest]
        public IEnumerator CoolDown_WhenStarted_InvokesCooldownStartEvent(){
            // Act
            _coolDownEvent.StartCoolDown(_testGameObject);

            // Wait one frame to ensure event processing
            yield return null;

            // Assert
            Assert.IsTrue(_startEventFired, "Cooldown start event should be triggered");
        }

        [UnityTest]
        public IEnumerator CoolDown_WhenCompleted_InvokesCooldownFinishedEvent(){
            // Arrange
            float testDuration = 0.5f; // Short duration for testing
            _coolDownEvent.CoolDownDuration = testDuration;

            // Act
            _coolDownEvent.StartCoolDown(_testGameObject);

            // Wait for slightly longer than the cooldown duration to ensure completion
            yield return new WaitForSeconds(testDuration + 0.1f);

            // Assert
            Assert.IsTrue(_coolDownEvent, "Cooldown finished event should be triggered");
        }

        [UnityTest]
        public IEnumerator CoolDown_WhenStartedMultipleTimes_HandlesCorrectly(){
            // Arrange
            float testDuration = 0.5f;
            _coolDownEvent.CoolDownDuration = testDuration;
            bool firstCooldownStarted = false;
            bool secondCooldownStarted = false;

            // Act
            // Start first cooldown
            _coolDownEvent.StartCoolDown(_testGameObject);
            firstCooldownStarted = _startEventFired;

            // Reset flag
            _startEventFired = false;

            // Try to start second cooldown immediately
            _coolDownEvent.StartCoolDown(_testGameObject);
            secondCooldownStarted = _startEventFired;

            yield return new WaitForSeconds(testDuration + 0.1f);

            // Assert
            Assert.IsTrue(firstCooldownStarted, "First cooldown should have started");
            Assert.IsFalse(secondCooldownStarted, "Second cooldown should not have started while first is active");
            Assert.IsTrue(_coolDownEvent, "Cooldown should have finished");
        }

        [UnityTest]
        public IEnumerator CoolDown_DuringCooldown_ReturnsCorrectActiveState(){
            // Arrange
            float testDuration = 1f;
            _coolDownEvent.CoolDownDuration = testDuration;
            bool duringCooldown;
            bool afterCooldown;

            // Act
            _coolDownEvent.StartCoolDown(_testGameObject);
            yield return new WaitForSeconds(0.1f);
            duringCooldown = _coolDownEvent.IsInCooldown;

            yield return new WaitForSeconds(testDuration);
            afterCooldown = _coolDownEvent.IsInCooldown;

            // Assert
            Assert.IsTrue(duringCooldown, "Cooldown should be active during the cooldown period");
            Assert.IsFalse(afterCooldown, "Cooldown should not be active after completion");
        }

        [Test]
        public void CoolDown_WithInvalidParameters_HandlesGracefully(){
            // Act & Assert
            Assert.DoesNotThrow(() => _coolDownEvent.StartCoolDown(null),
                "Should handle null MonoBehaviour gracefully");
        }


        // Helper class
        public class TestMonoBehaviour : MonoBehaviour{ }
    }

}