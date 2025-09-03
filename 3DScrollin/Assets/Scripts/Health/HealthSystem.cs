using System;

namespace Health
{
    public class HealthSystem : IHealthSystem
    {
        private readonly IHealthData _healthDataData;

        public HealthSystem(IHealthData healthDataData)
        {
            _healthDataData = healthDataData ?? throw new ArgumentNullException(nameof(healthDataData));
        }

        public bool TryTakeDamage(int damage)
        {
            // Implement just enough to pass the tests
            if (damage <= 0) return false;
            if (_healthDataData.CurrentHealth <= 0) return false;
            
            _healthDataData.CurrentHealth -= damage;
            return true;
        }

        public bool TryHeal(int amount)
        {
            if (amount <= 0) return false;
            if (_healthDataData.CurrentHealth >= _healthDataData.MaxHealth) return false;
            
            _healthDataData.CurrentHealth += amount;
            return true;
        }

        public void IncreaseMaxHealth(int amount)
        {
            _healthDataData.MaxHealth += amount;
        }
    }
}
