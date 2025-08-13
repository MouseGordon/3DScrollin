using GameEvent;
using Player.Gravity;
using Player.Jump;
using Player.Movement;
using Player.Stamina;
//using Rewind;
using UnityEngine;

namespace Player{
    [DisallowMultipleComponent][RequireComponent(typeof(CharacterController))]
    public partial class PlayerController : MonoBehaviour{
        public JumpSystem JumpSystem => _jumpSystem;
        public CoreCharacterData CoreCharacterData => coreCharacterData;
        [SerializeField] private CoreCharacterData coreCharacterData;
        [SerializeField] private TargetMovedGameEvent targetMovedGameEvent;
    
        private CharacterController _controller;
        private PlayerInputHandler _inputHandler;
        private MovementSystem _movementSystem;
        private StaminaSystem _staminaSystem;
        private GravitySystem _gravitySystem;
        private JumpSystem _jumpSystem;
       
    
        private void Awake()
        {
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
            _staminaSystem.OnStartStaminaCooldown -= StartStaminaCoolDown;
            _inputHandler.Dispose();
            _staminaSystem.Dispose();
        }

        private void StartStaminaCoolDown(){
            coreCharacterData.StaminaData.StaminaCoolDownEvent.StartCoolDown(this);
        }
    }

}