using GameEvent;
//using Rewind;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player{
    public class PlayerController : MonoBehaviour{
        [SerializeField] private CharacterController controller;
        [SerializeField] private float tiredSpeed = 5;
        [SerializeField] private float movementSpeed = 10;
        [SerializeField] private float sprintSpeed = 20;
        [SerializeField] private float jumpForce = 10;
        [SerializeField] private float gravityMultiplier = 3;
        [SerializeField] private int jumpsAllowed = 3;
        [SerializeField] private float stamina = 3;
        [SerializeField] private float staminaCooldown = 5;

        [SerializeField] private StaminaChangedEvent staminaEvent;
        [SerializeField] private CoolDownGameEvent coolDownEvent;
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
        private float _staminaTimer;
        private bool _sprintToggled = false;
        private bool _coolDownTriggered = false;
        private bool _exhausted = false;

        private const float c_gravity = -9.81f;

        
        private int _remainingJumps;
        private bool _isJumping;
        private float _fallMultiplier = 2.5f;
        private bool _rewinding;
        private void Awake(){
            _inputSystemActions = new InputSystem_Actions();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnDestroy(){
            coolDownEvent.EventAction -= StaminaCooldown;
            staminaEvent.EventAction -= StaminaEvent;
        }

        private void OnEnable(){
            _move = _inputSystemActions.Player.Move;
            _move.Enable();
            _currentSpeed = movementSpeed;
            _staminaTimer = stamina;

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

            staminaEvent.EventAction += StaminaEvent;
            coolDownEvent.EventAction += StaminaCooldown;
            coolDownEvent.StartCoolDownAction += StartStaminaCooldown;
            
            _remainingJumps = jumpsAllowed;
            _isJumping = false;
            
        }

        private void StartStaminaCooldown(){
            _exhausted = true;
            _coolDownTriggered = true;
        }

        private void OnRewindOnPerformed(InputAction.CallbackContext context){
            _rewinding = context.performed;
          
            if (!context.performed){
                return;
            }
            //recorder.UpdateRewindData(RewindRecorder.RecordState.Playing);
        }

        private void StaminaEvent(float stamina){
            if (_exhausted == false){
                return;
            }

            if (stamina < .6){
                return;
            }

            _exhausted = false;
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

            float speed = _exhausted ? tiredSpeed : _currentSpeed;
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

            if (IsSprinting() &&
                !_exhausted){
                _staminaTimer -= Time.deltaTime;
                staminaEvent.Raise(data: _staminaTimer / stamina);
                return;
            }

            if (!(stamina > _staminaTimer)){
                _staminaTimer = stamina;
                return;
            }

            _staminaTimer += Time.deltaTime;
            staminaEvent.Raise(data: _staminaTimer / stamina);
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

            if (_staminaTimer <= 0){
                coolDownEvent.StartCoolDown(this);
                return false;
            }

            return true;
        }

        private void StaminaCooldown(bool cooldownFinished){
            _coolDownTriggered = false;
        }
    }
}