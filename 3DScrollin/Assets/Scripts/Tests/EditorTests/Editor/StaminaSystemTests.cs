using System;
using GameEvent;
using NUnit.Framework;
using Player.Stamina;
using UnityEngine;

namespace Tests.EditorTests.Editor
{
    public class MockStaminaData : IStaminaData
    {
        public float Stamina { get; set; } = 10f;
        public float MaxStamina { get; set; } = 10f;
        public float StaminaRecoveryRate { get; set; } = 1f;
        public float StaminaDrainRate { get; set; } = 1f;
        public float ExhaustionThreshold { get; set; } = .6f;
        public bool IsExhausted { get; set; }
        public event Action<float> StaminaChangedActionEvent;
        public void InvokeStaminaChangedEvent(float value) => StaminaChangedActionEvent?.Invoke(value);
        public CoolDownGameEvent StaminaCoolDownEvent{ get; } = new();
    }

    [TestFixture]
    public class StaminaSystemTests
    {
        private StaminaSystem _staminaSystem;
        private MockStaminaData _mockData;
        private float _lastStaminaChangeValue;

        [SetUp]
        public void Setup()
        {
            _mockData = new MockStaminaData();
            _staminaSystem = new StaminaSystem(_mockData);
            _lastStaminaChangeValue = 0f;
            _mockData.StaminaChangedActionEvent += (value) => _lastStaminaChangeValue = value;
        }

        [Test]
        public void TryUseStamina_WhenEnoughStamina_DecreasesStamina()
        {
            // Arrange
            float initialStamina = _mockData.Stamina;
            float deltaTime = 1f;
            float staminaDecrease = _mockData.StaminaDrainRate * deltaTime;
            float expectedStamina = initialStamina - staminaDecrease;
            float expectedPercentage = expectedStamina / _mockData.MaxStamina;  // Change value should be the new percentage

            // Act
            _staminaSystem.TryUseStamina(deltaTime);

            // Assert
            Assert.That(_mockData.Stamina, Is.EqualTo(expectedStamina).Within(0.01f));
            Assert.That(_lastStaminaChangeValue, Is.EqualTo(expectedPercentage).Within(0.01f));
        }


        [Test]
        public void RecoverStamina_WhenBelowMax_IncreasesStamina()
        {
            // Arrange
            _mockData = new MockStaminaData
            {
                StaminaRecoveryRate = 0.6f,
                MaxStamina = 100f  // Ensure max stamina is set before system creation
            };
            _staminaSystem = new StaminaSystem(_mockData);  // Create system after setting up data
    
            _mockData.Stamina = 50f;  // Set initial stamina after system creation
            float deltaTime = 1f;
    
            // Act
            _staminaSystem.RecoverStamina(deltaTime);
    
            // Assert
            float expectedChange = 0.6f;
            float expectedStamina = 50f + expectedChange;
            Assert.That(_mockData.Stamina, Is.EqualTo(expectedStamina).Within(0.01f));
        }
        
        [Test]
        public void RecoverStamina_WhenAtMax_DoesNotIncreaseStamina()
        {
            _mockData.Stamina = _mockData.MaxStamina;
            float deltaTime = 1f;

            _staminaSystem.RecoverStamina(deltaTime);

            Assert.That(_mockData.Stamina, Is.EqualTo(_mockData.MaxStamina));
            Assert.That(_lastStaminaChangeValue, Is.EqualTo(0f));
        }

        [Test]
        public void TryUseStamina_WhenAtZero_SetsExhausted(){
            _mockData.IsExhausted = false;
            _mockData.Stamina = 0f;
            float deltaTime = 1f;

            _staminaSystem.TryUseStamina(deltaTime);
            _staminaSystem.TryUseStamina(deltaTime);
            Assert.That(_mockData.Stamina,
                Is.EqualTo(0));
            Assert.That(_mockData.IsExhausted, Is.True);
        }

        [Test]
        public void RecoverStamina_WhenExhaustedAndAboveThreshold_ResetsExhausted()
        {
            _mockData.IsExhausted = true;
            _mockData.Stamina = _mockData.ExhaustionThreshold*_mockData.MaxStamina + 1f;
            float deltaTime = 1f;

            _staminaSystem.RecoverStamina(deltaTime);

            Assert.That(_mockData.IsExhausted, Is.False);
        }

        [Test]
        public void TryUseStamina_WhenNotEnoughStamina_ClampsToZero()
        {
            // Arrange
            _mockData = new MockStaminaData
            {
                StaminaDrainRate = 10f,  // Set drain rate higher than initial stamina
                MaxStamina = 10f,
                Stamina = 5f
            };
            _staminaSystem = new StaminaSystem(_mockData);
    
            float deltaTime = 1f;  // With drain rate of 10, this would try to drain 10 stamina

            // Act
            _staminaSystem.TryUseStamina(deltaTime);

            // Assert
            Assert.That(_mockData.Stamina, Is.EqualTo(0f));
        }


        [Test]
        public void RecoverStamina_WhenRecovering_ClampsToMaxStamina()
        {
            _mockData.Stamina = _mockData.MaxStamina - 5f;
            float deltaTime = 1f;

            _staminaSystem.RecoverStamina(deltaTime);

            Assert.That(_mockData.Stamina, Is.LessThanOrEqualTo(_mockData.MaxStamina));
        }
    }
}
