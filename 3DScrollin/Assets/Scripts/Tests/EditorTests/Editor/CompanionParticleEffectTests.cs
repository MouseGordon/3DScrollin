using System;
using System.Diagnostics;
using Companion;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.EditorTests.Editor{
    [TestFixture]
    public class CompanionParticleEffectTests{
        private CompanionParticleEffect _effect;
        private CompanionData _data;

        [SetUp]
        public void Setup(){
            _data = ScriptableObject.CreateInstance<CompanionData>();
            _effect = new CompanionParticleEffect(_data);
        }

        [TearDown]
        public void TearDown(){
            if (_data != null){
                Object.DestroyImmediate(_data);
            }
        }

        [Test]
        public void Constructor_NullData_ThrowsArgumentNullException(){
            Assert.Throws<ArgumentNullException>(() => new CompanionParticleEffect(null));
        }

        [TestCase(0.1f, 0f, Description = "Below threshold, moving right")]
        [TestCase(-0.1f, 0f, Description = "Below threshold, moving left")]
        [TestCase(0f, 2f, Description = "Vertical only")]
        [TestCase(0f, 0f, Description = "No movement")]
        public void BelowThreshold_ReturnsZeroOrbital(float x, float y){
            var result = _effect.UpdateEffect(new Vector3(x, y));
            Assert.That(result.constant, Is.EqualTo(0f));
        }

        [TestCase(1f, 0f, -1.5f, Description = "Right movement")]
        [TestCase(-1f, 0f, 1.5f, Description = "Left movement")]
        [TestCase(2f, 2f, -1.5f, Description = "Up-right diagonal")]
        [TestCase(-2f, 2f, 1.5f, Description = "Up-left diagonal")]
        public void AboveThreshold_ReturnsCorrectOrbitalDirection(float x, float y, float expectedOrbital){
            var result = _effect.UpdateEffect(new Vector2(x, y));
            Assert.That(result.constant, Is.EqualTo(expectedOrbital),
                $"Moving in direction ({x},{y}) should return {expectedOrbital}");
        }

        [Test]
        public void ExactlyAtThreshold_ReturnsZeroOrbital(){
            var result = _effect.UpdateEffect(new Vector3(0.5f, 0f));
            Assert.That(result.constant, Is.EqualTo(0f), "At threshold should return zero orbital");
        }

        [Test]
        public void WithCustomValues_RespectsThresholdAndMultiplier(){
            var testData = new TestCompanionData {
                VelocityThreshold = 1.0f,
                OrbitalMultiplier = 2.0f
            };
            var effect = new CompanionParticleEffect(testData);

            var result = effect.UpdateEffect(new Vector3(1.5f, 0f));
            Assert.That(result.constant, Is.EqualTo(-2.0f), "Should use custom multiplier value");
        }

        [Test]
        public void MultipleUpdates_MaintainsConsistentBehavior()
        {
            var firstResult = _effect.UpdateEffect(new Vector2(1f, 0f));
            var secondResult = _effect.UpdateEffect(new Vector2(1f, 0f));
            Assert.That(secondResult.constant, Is.EqualTo(firstResult.constant));
        }
        
        [Test]
        public void PerformanceTest_UpdateEffect()
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 10000; i++)
            {
                _effect.UpdateEffect(new Vector2(1f, 1f));
            }
            sw.Stop();
            Assert.That(sw.ElapsedMilliseconds, Is.LessThan(100), "Should process 10k updates under 100ms");
        }


    }

    public class TestCompanionData : ICompanionData{
        public float VelocityThreshold{ get; set; } = 0.5f;
        public float OrbitalMultiplier{ get; set; } = 1.5f;
    }
}