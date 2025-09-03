using NUnit.Framework;
using Health;

namespace Tests.EditorTests.Editor{
    // Minimal stub - only tracks calls, no logic
    public class HealthDataDataStub : IHealthData{
        public int CurrentHealth{ get; set; }
        public int MaxHealth{ get; set; }
        
    }

    [TestFixture]
    public class HealthSystemTests{
        private HealthDataDataStub _healthDataData;
        private IHealthSystem _healthSystem;

        [SetUp]
        public void Setup(){
            _healthDataData = new HealthDataDataStub();
            
            _healthSystem = new HealthSystem(_healthDataData);
        }

        [Test]
        public void TryTakeDamage_WithPositiveDamage_ReturnsTrue(){
            // Arrange
            _healthDataData.CurrentHealth = 50;

            // Act
            var result = _healthSystem.TryTakeDamage(10);

            // Assert - Testing SYSTEM logic
            Assert.That(result, Is.True);
            Assert.That(_healthDataData.CurrentHealth == 40);
        }

        [Test]
        public void TryTakeDamage_WhenDead_ReturnsFalse(){
            // Arrange
            _healthDataData.CurrentHealth = 0;

            // Act
            var result = _healthSystem.TryTakeDamage(10);

            // Assert - Testing SYSTEM's decision logic
            Assert.That(result, Is.False);
            Assert.That(_healthDataData.CurrentHealth == 0);
        }

        [Test]
        public void TryTakeDamage_WithInvalidDamage_ReturnsFalse(){
            // Act & Assert - Testing SYSTEM validation
            Assert.That(_healthSystem.TryTakeDamage(0), Is.False);
            Assert.That(_healthSystem.TryTakeDamage(-5), Is.False);
        }

        [Test]
        public void TryTakeDamage_WhenHasHealthTakeDamage_TakeDamage(){
            // Arrange
            _healthDataData.CurrentHealth = 50;

            // Act
            _healthSystem.TryTakeDamage(10);
            
            Assert.That(_healthDataData.CurrentHealth == 40);
        }

        [Test]
        public void TryHeal_WhenAtMaxHealth_ReturnsFalse(){
            // Arrange
            _healthDataData.CurrentHealth = 100;
            _healthDataData.MaxHealth = 100;

            // Act
            var result = _healthSystem.TryHeal(10);

            // Assert - Testing SYSTEM logic
            Assert.That(result, Is.False);
            Assert.That(_healthDataData.CurrentHealth == 100);
        }

        [Test]
        public void TryHeal_WhenBelowMax_ReturnsTrue(){
            // Arrange
            int currentHealth = 50;
            _healthDataData.CurrentHealth = currentHealth;
            _healthDataData.MaxHealth = 100;

            // Act
            var result = _healthSystem.TryHeal(10);

            // Assert - Testing SYSTEM logic
            Assert.That(result, Is.True);
            Assert.That(_healthDataData.CurrentHealth == 60);

        }

        [Test]
        public void IncreaseMaxHealth_IncreasesMaxHealth(){
            //Arrange
            int startingMaxHealth = 50;
            _healthDataData.MaxHealth = startingMaxHealth;
            
            // Act
            _healthSystem.IncreaseMaxHealth(25);

            // Assert - Testing SYSTEM delegates
            Assert.That(_healthDataData.MaxHealth >  startingMaxHealth);
        }
    }
}
