namespace Health{
    public interface IHealthSystem{
        bool TryTakeDamage(int damage);
        bool TryHeal(int amount);
        void IncreaseMaxHealth(int amount);
    }
}