using System;
using GameEvent;
using GameStateSystems;
using Player.Gravity;
using Player.Jump;
using Player.Movement;
using Player.Stamina;
using SaveSystems;
//using Rewind;
using UnityEngine;

namespace Player{
    [DisallowMultipleComponent][RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour{
        [SerializeField] private CoreCharacterData coreCharacterData;
        [SerializeField] private TargetMovedGameEvent targetMovedGameEvent;
    
        private CharacterController _controller;
        private PlayerInputHandler _inputHandler;
        private MovementSystem _movementSystem;
        private StaminaSystem _staminaSystem;
        private GravitySystem _gravitySystem;
        private JumpSystem _jumpSystem;
        private bool _initialized;
    
        private void Awake()
        {
           GameManager.OnGameObjectsInitialize += Initialize;
           _initialized = false;
        }

        private void Initialize(){
            _controller = GetComponent<CharacterController>();
            
            _staminaSystem = new StaminaSystem(coreCharacterData.StaminaData);
            _movementSystem = new MovementSystem(coreCharacterData.MovementData);
            _gravitySystem = new GravitySystem(coreCharacterData.GravityData);
            _jumpSystem = new JumpSystem(
                jumpData:coreCharacterData.JumpData, 
                gravity:coreCharacterData.GravityData.GravityForce,
                groundedGraivty:coreCharacterData.GravityData.GroundedGravity);
            
            _inputHandler = new PlayerInputHandler(
                onMove: direction => _movementSystem.SetMoveDirection(direction),
                onJump: _jumpSystem.HandleJumpInput,
                onSprint: _ => _movementSystem.ToggleSprint()
            );
            
            _staminaSystem.OnStartStaminaCooldown += StartStaminaCoolDown;
            targetMovedGameEvent.EventAction?.Invoke(transform);
            _initialized = true;
        }

        private void Update()
        {
            if (!_initialized){
                return;
            }
            UpdateStamina();
        }
    
        private void UpdateStamina()
        {
            if (coreCharacterData.StaminaData.StaminaCoolDownEvent.IsInCooldown)
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

            if (!_initialized){
                return;
            }
            // Calculate gravity using gravity system
            //
            float gravityVelocity = _gravitySystem.CalculateGravity(_controller.isGrounded);
        
            // Calculate jump velocity - we only need the Y component
            //
            Vector3 jumpMovement = _jumpSystem.CalculateJumpVelocity(_controller.isGrounded);
            float combinedVerticalVelocity = jumpMovement.y + gravityVelocity * Time.fixedDeltaTime;
            
            // Apply horizontal movement with the combined vertical velocity
            //
            Vector3 finalMovement = _movementSystem.Move(combinedVerticalVelocity, coreCharacterData.StaminaData.IsExhausted);
            _controller.Move(finalMovement);
        }

        private void OnDestroy()
        {
            GameManager.OnGameObjectsInitialize -= Initialize;
            _staminaSystem.OnStartStaminaCooldown -= StartStaminaCoolDown;
            _inputHandler.Dispose();
            _staminaSystem.Dispose();
        }

        private void StartStaminaCoolDown(){
            coreCharacterData.StaminaData.StaminaCoolDownEvent.StartCoolDown(this);
        }
    }

}