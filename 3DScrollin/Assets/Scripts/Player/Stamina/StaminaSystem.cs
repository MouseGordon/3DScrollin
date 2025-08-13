using System;
using UnityEngine;

namespace Player.Stamina{
    public class StaminaSystem:IDisposable{
        private readonly float _maxStamina;
        private readonly float _staminaRecoveryRate;
        private readonly float _staminaDrainRate;
        private readonly float _exhaustionThreshold;

        private bool _isCoolingDown;

        public event Action OnStartStaminaCooldown;

        private readonly IStaminaData _staminaData;
        private readonly Action<bool> OnCoolDownComplete;
        
        public StaminaSystem(IStaminaData staminaData){
            _staminaData = staminaData ?? throw new ArgumentNullException(nameof(staminaData));

            _maxStamina = staminaData.MaxStamina;
            _staminaRecoveryRate = staminaData.StaminaRecoveryRate;
            _staminaDrainRate = staminaData.StaminaDrainRate;
            
            _exhaustionThreshold = _staminaData.ExhaustionThreshold;
            _staminaData.Stamina = _maxStamina;
          
            OnCoolDownComplete = (value) => { _isCoolingDown = !value; };
            _staminaData.StaminaCoolDownEvent.EventAction += OnCoolDownComplete;
            
            _staminaData.IsExhausted = false;

        }
      
        public void RecoverStamina(float deltaTime){
            if (_isCoolingDown){
                return;
            }
            
            var newStamina = Mathf.Min(_staminaData.Stamina + (_staminaRecoveryRate * deltaTime), _maxStamina);
            _staminaData.Stamina = newStamina;
    
            var staminaPercentage = newStamina / _maxStamina;
            if (Mathf.Approximately(_staminaData.Stamina, _maxStamina)){
                return;
            }
            _staminaData.InvokeStaminaChangedEvent(staminaPercentage);

            if (newStamina <= _exhaustionThreshold * _maxStamina)
            {
                return;
            }
            _staminaData.IsExhausted = false;
        }

        public bool TryUseStamina(float deltaTime){
            if (_staminaData.IsExhausted || _isCoolingDown){
                return false;
            }
            
            if (_staminaData.Stamina <= 0){
                _staminaData.Stamina = 0;
                OnStartStaminaCooldown?.Invoke();
                _staminaData.IsExhausted = true;
                _isCoolingDown = true;
                return false;
            }
            
            var newStamina = _staminaDrainRate * deltaTime;
            _staminaData.Stamina -= newStamina;
            _staminaData.InvokeStaminaChangedEvent(_staminaData.Stamina/_maxStamina);
            return true;
        }

        public void Dispose(){
            _staminaData.StaminaCoolDownEvent.EventAction -= OnCoolDownComplete;
        }
    }

}
