using System;
using UnityEngine;

namespace Player{
    public class StaminaSystem:IDisposable{
        private readonly float _maxStamina;
        private readonly float _staminaRecoveryRate;
        private readonly float _staminaDrainRate;
        private readonly float _exhaustionThreshold;

        private bool _isCoolingDown;

        public event Action<float> OnStaminaChanged;
        public event Action OnRecovered;

        private readonly IPlayerCoreData _playerCoreData;
        private readonly MonoBehaviour _monoBehaviour;
        private readonly Action<bool> OnCoolDownComplete;
        
        public StaminaSystem(IPlayerCoreData playerCoreData,MonoBehaviour monoBehaviour){
            _monoBehaviour = monoBehaviour;
            _playerCoreData = playerCoreData;
            _maxStamina = playerCoreData.MaxStamina;
            _staminaRecoveryRate = playerCoreData.StaminaRecoveryRate;
            _staminaDrainRate = playerCoreData.StaminaDrainRate;
            
            _exhaustionThreshold = _playerCoreData.ExhaustionThreshold;
            _playerCoreData.Stamina = _maxStamina;
          
            OnCoolDownComplete = (value) => { _isCoolingDown = !value; };
            
            _playerCoreData.StaminaCoolDownEvent.EventAction += OnCoolDownComplete;
            _playerCoreData.IsExhausted = false;

        }
      
        public void RecoverStamina(float deltaTime){
            if (_isCoolingDown){
                return;
            }
            
            var newStamina = Mathf.Min(_playerCoreData.Stamina + (_staminaRecoveryRate * deltaTime), _maxStamina);
            _playerCoreData.Stamina = newStamina;
    
            var staminaPercentage = newStamina / _maxStamina;
            _playerCoreData.InvokeStanimaChangedEvent(staminaPercentage);

            if (newStamina <= _exhaustionThreshold * _maxStamina)
            {
                return;
            }
            _playerCoreData.IsExhausted = false;
            OnRecovered?.Invoke();

        }

        public bool TryUseStamina(float deltaTime){
            if (_playerCoreData.IsExhausted || _isCoolingDown){
                return false;
            }

            var newStamina = _staminaDrainRate * deltaTime;
            _playerCoreData.Stamina -= newStamina;
            _playerCoreData.InvokeStanimaChangedEvent(_playerCoreData.Stamina/_maxStamina);

            if (_playerCoreData.Stamina <= 0){
                _playerCoreData.StaminaCoolDownEvent.StartCoolDown(_monoBehaviour);
                _playerCoreData.IsExhausted = true;
                _isCoolingDown = true;
                return false;
            }

            return true;
        }

        public void Dispose(){
            _playerCoreData.StaminaCoolDownEvent.EventAction -= OnCoolDownComplete;
        }
    }

}
