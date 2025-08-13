using System;
using GameEvent;

namespace Player.Stamina{
    public interface IStaminaData{
            event Action<float> StaminaChangedActionEvent;
            float MaxStamina { get; }
            float Stamina { get; set; }
            float StaminaRecoveryRate { get; }
            float StaminaDrainRate { get; }
            float ExhaustionThreshold { get; }
            bool IsExhausted { get; set; }
            void InvokeStaminaChangedEvent(float staminaChanges);
            CoolDownGameEvent StaminaCoolDownEvent { get; }
    }
}