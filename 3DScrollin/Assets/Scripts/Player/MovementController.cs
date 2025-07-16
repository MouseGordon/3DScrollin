using GameEvent;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player{
    public class MovementController : MonoBehaviour{
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
        [SerializeField] private PlayerMovedGameEvent playerMovedGameEvent;

        private InputSystem_Actions _inputSystemActions;
        private InputAction _move;
        private InputAction _jump;
        private InputAction _fire;
        private InputAction _sprint;

        private float _velocity;

        private Vector2 _moveDirection;
        private float _currentSpeed;
        private float _staminaTimer;
        private bool _sprintToggled = false;
        private bool _coolDownTriggered = false;
        private bool _exhausted = false;

        private const float c_gravity = -9.81f;

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
            _move.performed += MoveOnperformed;
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
            
            staminaEvent.EventAction += StaminaEvent;
            coolDownEvent.EventAction += StaminaCooldown;
        }

        private void MoveOnperformed(InputAction.CallbackContext obj){
           // playerMovedGameEvent.EventAction.Invoke(transform.position);
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
        }

        // Update is called once per frame
        void Update(){
            _moveDirection = _move.ReadValue<Vector2>();
            CalculateStamina();
            _velocity = c_gravity * gravityMultiplier * Time.fixedDeltaTime;
            
        }

        private void FixedUpdate(){
            float speed = _exhausted ? tiredSpeed : _currentSpeed;
            controller.Move(new Vector3(x: _moveDirection.x, y: _velocity, z: 0) * speed * Time.fixedDeltaTime);
        }

        private void Jump(InputAction.CallbackContext context){
            if (!controller.isGrounded){
                return;
            }
            
            controller.Move(new Vector3(0, jumpForce, 0));
        }

        private void CalculateStamina(){
            if (_coolDownTriggered){
                return;
            }

            if (IsSprinting() && !_exhausted){
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
            if (!cooldownFinished){
                _exhausted = true;
                _coolDownTriggered = true;
                return;
            }
            _coolDownTriggered = false;
        }
    }
}