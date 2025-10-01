namespace Health{
    public interface IDamageable{
        bool TryTakeDamage(int damage);
        bool TryHeal(int amount);
    }
}