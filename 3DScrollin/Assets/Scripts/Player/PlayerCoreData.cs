using System;
using GameEvent;
using UnityEngine;

namespace Player{
    [CreateAssetMenu(fileName = "PlayerCoreData", menuName = "Scriptable Objects/PlayerCoreData")]
    public class PlayerCoreData : ScriptableObject,IPlayerCoreData{
        public event Action<float> StaminaChangedActionEvent;
        public float MaxStamina => maxStamina;
        public float Stamina{
            get => stamina;
            set => stamina = value;
        }

        public float StaminaRecoveryRate => staminaRecoveryRate;

        public float Health{
            get => health;
            set => health = value;
        }

        public float MaxHealth => maxHealth;
        public float StaminaDrainRate => staminaDrainRate;
        public CoolDownGameEvent StaminaCoolDownEvent => staminaCoolDownEvent;
        public bool IsExhausted{ get; set; }
        public float ExhaustionThreshold => exhaustionThreshold;
        
        [SerializeField] private float health;
        [SerializeField] private float maxHealth;
        [SerializeField] private float stamina;
        [SerializeField] private float maxStamina;
        [SerializeField] private float staminaRecoveryRate;
        [SerializeField] private float staminaDrainRate;
        [SerializeField] private CoolDownGameEvent staminaCoolDownEvent;
        [SerializeField] private float exhaustionThreshold;

        public void InvokeStanimaChangedEvent(float staminaChanges){
            StaminaChangedActionEvent?.Invoke(staminaChanges);
        }
    }

    public interface IPlayerCoreData{
        event Action<float> StaminaChangedActionEvent;
        float MaxStamina { get; }
        float Stamina { get; set; }
        float StaminaRecoveryRate { get; }
        float Health { get; set; }
        float MaxHealth { get; }
        float StaminaDrainRate { get; }
        CoolDownGameEvent StaminaCoolDownEvent { get; }
        bool IsExhausted{ get; set; }
        float ExhaustionThreshold { get; }
        void InvokeStanimaChangedEvent(float staminaChanges);

    }
}