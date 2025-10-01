using UnityEngine;

namespace Health{
    [CreateAssetMenu(fileName = "PlayerHealthData", menuName = "Scriptable Objects/Health/PlayerHealth", order = 0)]
    public class HealthData : ScriptableObject, IHealthData{
        [SerializeField] private int _maxHealth;

        public int CurrentHealth{ get; set; }

        public int MaxHealth{
            get => _maxHealth;
            set => _maxHealth = value;
        }
    }
}