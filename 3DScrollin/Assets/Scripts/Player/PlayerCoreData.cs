using System;
using GameEvent;
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
        public PlayerStaminaData StaminaData => staminaData;
        public PlayerMovementData MovementData => movementData;


        [SerializeField] private float health;
        [SerializeField] private float maxHealth;
        [SerializeField] private PlayerStaminaData staminaData;
        [SerializeField] private PlayerMovementData movementData;
 
    }

    public interface ICoreCharacterData{
        float Health { get; set; }
        float MaxHealth { get; }
    }
}