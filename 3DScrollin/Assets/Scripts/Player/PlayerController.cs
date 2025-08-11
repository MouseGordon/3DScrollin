using System;
using GameEvent;
//using Rewind;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player{
    public class PlayerController : MonoBehaviour{
        [SerializeField] private CharacterController controller;
        [SerializeField] private PlayerCoreData playerCoreData;
        [SerializeField] private float tiredSpeed = 5;
        [SerializeField] private float movementSpeed = 10;
        [SerializeField] private float sprintSpeed = 20;
        [SerializeField] private float jumpForce = 10;
        [SerializeField] private float gravityMultiplier = 3;
        [SerializeField] private int jumpsAllowed = 3;
        
        [SerializeField] private TargetMovedGameEvent targetMovedGameEvent;
        [SerializeField] private JumpController jumpController;
        //[SerializeField] private RewindRecorder recorder;
        
        private InputSystem_Actions _inputSystemActions;
        private InputAction _move;
        private InputAction _jump;
        private InputAction _fire;
        private InputAction _sprint;
        private InputAction _rewind;

        private float _velocity;

        private Vector2 _moveDirection;
        private float _currentSpeed;
        private bool _sprintToggled = false;
        private bool _coolDownTriggered = false;
        
        private bool _rewinding;
        private StaminaSystem _staminaSystem;
        
        private void Awake(){
            _inputSystemActions = new InputSystem_Actions();
            _staminaSystem = new StaminaSystem(playerCoreData:playerCoreData,this);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnDestroy(){
            _staminaSystem.Dispose();
        }

        private void OnEnable(){
            _move = _inputSystemActions.Player.Move;
            _move.Enable();
            _currentSpeed = movementSpeed;

            _jump = _inputSystemActions.Player.Jump;
            _jump.Enable();
            _jump.performed += Jump;

            _fire = _inputSystemActions.Player.Attack;
            _fire.Enable();

            _sprint = _inputSystemActions.Player.Sprint;
            _sprint.Enable();
            _sprint.performed += SprintOnPerformed;

            /*_rewind = _inputSystemActions.Player.Rewind;
            _rewind.Enable();
            _rewind.performed += OnRewindOnPerformed;
            _rewind.canceled += OnRewindOnPerformed;*/
            
        }

        private void StartStaminaCooldown(){
            _coolDownTriggered = true;
        }

        private void OnRewindOnPerformed(InputAction.CallbackContext context){
            _rewinding = context.performed;
          
            if (!context.performed){
                return;
            }
            //recorder.UpdateRewindData(RewindRecorder.RecordState.Playing);
        }

        private void SprintOnPerformed(InputAction.CallbackContext context){
            if (Mathf.Approximately(_currentSpeed, sprintSpeed)){
                _currentSpeed = movementSpeed;
                _sprintToggled = false;
                return;
            }

            _sprintToggled = true;

            _currentSpeed = sprintSpeed;
        }

        private void OnDisable(){
            _move.Disable();

            _jump.Disable();
            _jump.performed -= Jump;

            _fire.Disable();
            _sprint.performed -= SprintOnPerformed;
            
            _rewind.Disable();
            _rewind.performed -= OnRewindOnPerformed;
            _rewind.canceled -= OnRewindOnPerformed;
        }

        // Update is called once per frame
        void Update(){
            if (_rewinding){
                return;
            }
            _moveDirection = _move.ReadValue<Vector2>();
            if (_moveDirection.x > 0){
                targetMovedGameEvent.EventAction?.Invoke(transform);
            }

            CalculateStamina();
            //recorder.UpdateRewindData(RewindRecorder.RecordState.Record);
            
        }

        private void FixedUpdate(){
            float speed = playerCoreData.IsExhausted ? tiredSpeed : _currentSpeed;
            controller.Move(new Vector3(_moveDirection.x, jumpController.Velocity, 0) * speed * Time.fixedDeltaTime);
        }

        private void Jump(InputAction.CallbackContext context)
        {
            jumpController.Jump(context);
        }
        
        private void CalculateStamina(){
           
            if (_coolDownTriggered){
                return;
            }
           
            var deltaTime = Time.deltaTime;

            if (IsSprinting() &&
                !playerCoreData.IsExhausted){
                _staminaSystem.TryUseStamina(deltaTime);
                return;
            }
            
            _staminaSystem.RecoverStamina(deltaTime);
        }

        private bool IsSprinting(){
            // is sprint toggled
            //
            if (!_sprintToggled){
                return false;
            }

            // if we aren't in motion, we aren't sprinting
            //
            if (_moveDirection.x == 0){
                return false;
            }

            // if we are out of stamina and the cooldown timer is active, we aren't sprinting
            //
            if (_coolDownTriggered){
                return false;
            }


            return true;
        }
    }
}