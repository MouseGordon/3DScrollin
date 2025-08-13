using System;
using GameEvent;
using UnityEngine;

namespace Player.Stamina{
    [CreateAssetMenu(fileName = "PlayerStaminaData", menuName = "Scriptable Objects/PlayerStaminaData")]
    public class PlayerStaminaData:ScriptableObject,IStaminaData{
        public event Action<float> StaminaChangedActionEvent;
        public float Stamina{
            get => _stamina;
            set => _stamina = value;
        }
        public bool IsExhausted{ get; set; }
        public float MaxStamina => _maxStamina;
        public float StaminaRecoveryRate => _staminaRecoveryRate;
        public float StaminaDrainRate => _staminaDrainRate;
        public float ExhaustionThreshold => _exhaustionThreshold;
     
        public void InvokeStaminaChangedEvent(float staminaChanges){
            StaminaChangedActionEvent?.Invoke(staminaChanges);
        }

        public CoolDownGameEvent StaminaCoolDownEvent => _staminaCoolDownEvent;

        [SerializeField] private float _stamina;
        [SerializeField] private float _maxStamina;
        [SerializeField] private float _staminaRecoveryRate;
        [SerializeField] private float _staminaDrainRate;
        [SerializeField] private float _exhaustionThreshold;
        [SerializeField] private CoolDownGameEvent _staminaCoolDownEvent;
    }
}