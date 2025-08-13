using System;
using GameEvent;
using Player.Gravity;
using Player.Jump;
using Player.Movement;
using Player.Stamina;
using UnityEngine;

namespace Player{
    [CreateAssetMenu(fileName = "PlayerCoreData", menuName = "Scriptable Objects/PlayerCoreData")]
    public class CoreCharacterData : ScriptableObject,ICoreCharacterData{
      
        public float Health{
            get => health;
            set => health = value;
        }

        public float MaxHealth => maxHealth;
        public IStaminaData StaminaData => staminaData;

        public IGravityData GravityData => gravityData;

        public IJumpData JumpData => jumpData;

        public IMovementData MovementData => movementData;


        [SerializeField] private float health;
        [SerializeField] private float maxHealth;
        [SerializeField] private PlayerStaminaData staminaData;
        [SerializeField] private PlayerMovementData movementData;
        [SerializeField] private PlayerGravityData gravityData;
        [SerializeField] private PlayerJumpData jumpData;
    }

    public interface ICoreCharacterData{
        float Health { get; set; }
        float MaxHealth { get; }
        IMovementData MovementData { get; }
        IStaminaData StaminaData { get; }
        IGravityData GravityData{ get; }
        IJumpData JumpData{ get; }
    }
}