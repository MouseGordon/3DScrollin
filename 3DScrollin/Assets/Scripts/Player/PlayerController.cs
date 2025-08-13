using GameEvent;
using Player.Movement;
using Player.Stamina;
//using Rewind;
using UnityEngine;

namespace Player{
    public class PlayerController : MonoBehaviour{
        [SerializeField] private CharacterController controller;
        [SerializeField] private CoreCharacterData coreCharacterData;
        [SerializeField] private JumpController jumpController;
        [SerializeField] private TargetMovedGameEvent targetMovedGameEvent;
    
        private MovementSystem _movementSystem;
        private PlayerInputHandler _inputHandler;
        private StaminaSystem _staminaSystem;
        private bool _coolDownTriggered;
    
        private void Awake()
        {
            _inputHandler = new PlayerInputHandler(
                onMove: direction => _movementSystem.SetMoveDirection(direction),
                onJump: jumpController.Jump,
                onSprint: _ => _movementSystem.ToggleSprint()
            );
            
            _staminaSystem = new StaminaSystem(coreCharacterData.StaminaData);
            _movementSystem = new MovementSystem(coreCharacterData.MovementData);
           
            _staminaSystem.OnStartStaminaCooldown += StartStaminaCoolDown;
        }
    
        private void Update()
        {
            var moveDirection = _inputHandler.GetMoveInput();
            if (moveDirection.x > 0)
            {
                targetMovedGameEvent.EventAction?.Invoke(transform);
            }
        
            UpdateStamina();
        }
    
        private void UpdateStamina()
        {
            if (_coolDownTriggered)
                return;
            
            var deltaTime = Time.deltaTime;

            if (_movementSystem.IsSprinting && !coreCharacterData.StaminaData.IsExhausted)
            {
                _staminaSystem.TryUseStamina(deltaTime);
                return;
            }
        
            _staminaSystem.RecoverStamina(deltaTime);
        }

        private void FixedUpdate(){
            
            controller.Move(jumpController.ApplyPhysics());
            jumpController.UpdateJumpState();
            controller.Move(_movementSystem.Move(jumpController.Velocity, coreCharacterData.StaminaData.IsExhausted));
        }

        private void OnDestroy()
        {
            _staminaSystem.OnStartStaminaCooldown -= StartStaminaCoolDown;
            _inputHandler.Dispose();
            _staminaSystem.Dispose();
        }

        private void StartStaminaCoolDown(){
            coreCharacterData.StaminaData.StaminaCoolDownEvent.StartCoolDown(this);
        }
    }

}