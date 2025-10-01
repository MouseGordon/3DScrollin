using System;
using UnityEngine;

namespace Health{
    public class BaseDamageComponent : MonoBehaviour, IDamageable{
        [SerializeField] HealthData healthData;

        private HealthSystem _healthSystem;

        private void Awake(){
            _healthSystem = new HealthSystem(healthData);
        }
        
        public bool TryTakeDamage(int damage){
           return _healthSystem.TryTakeDamage(damage);
        }
        
        public bool TryHeal(int amount){
           return _healthSystem.TryHeal(amount);
        }
    }
}