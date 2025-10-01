using System;

namespace Health
{
    public class HealthSystem : IHealthSystem
    {
        private readonly IHealthData _healthData;

        public HealthSystem(IHealthData healthDataData)
        {
            _healthData = healthDataData ?? throw new ArgumentNullException(nameof(healthDataData));
        }

        public bool TryTakeDamage(int damage)
        {
            // Implement just enough to pass the tests
            if (damage <= 0) return false;
            if (_healthData.CurrentHealth <= 0) return false;
            
            _healthData.CurrentHealth -= damage;
            return true;
        }

        public bool TryHeal(int amount)
        {
            if (amount <= 0) return false;
            if (_healthData.CurrentHealth >= _healthData.MaxHealth) return false;
            
            _healthData.CurrentHealth += amount;
            return true;
        }

        public void IncreaseMaxHealth(int amount)
        {
            _healthData.MaxHealth += amount;
        }
    }
}
