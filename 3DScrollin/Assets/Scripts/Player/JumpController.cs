using UnityEngine;
using UnityEngine.InputSystem;

namespace Player{
    public class JumpController : MonoBehaviour{
        public float Velocity => _velocity;
        
        public enum JumpState{
            Grounded,
            JumpInitiation,
            Rising,
            Apex,
            Falling,
            Landing,
            DoubleJumping
        }

        [Header("Jump Properties")] [SerializeField]
        private float jumpForce = 10f;

        [SerializeField] private float minApexThreshold = 0.1f; // Velocity near zero
        [SerializeField] private float fastFallThreshold = -8f;
        [SerializeField] private float landingRecoveryTime = 0.1f;
        [SerializeField] private float doubleJumpForce = 8f; 
        [SerializeField] private CharacterController controller;
        
        private const float c_gravity = -9.81f;
        private JumpState _currentJumpState = JumpState.Grounded;
        private float _velocity;
        private float _landingTimer;
        private bool _isJumpButtonHeld;
        private bool _hasDoubleJumped; // Track if double jump was used


        // For debugging/animation
        public JumpState CurrentJumpState => _currentJumpState;

        void FixedUpdate(){
            Debug.Log($"IsGrounded: {controller.isGrounded} State: {_currentJumpState}");
            ApplyJumpPhysics();
            UpdateJumpState();
            HandleStateEffects();
        }

        private void UpdateJumpState(){
            bool isGrounded = controller.isGrounded;
            float verticalVelocity = _velocity;

            if (isGrounded)
            {
                _currentJumpState = JumpState.Grounded;
                _hasDoubleJumped = false;
            }
            else // In air
            {
                switch (_currentJumpState)
                {
                    case JumpState.JumpInitiation:
                    case JumpState.DoubleJumping:
                        _currentJumpState = JumpState.Rising;
                        break;

                    case JumpState.Rising:
                        if (Mathf.Abs(verticalVelocity) <= minApexThreshold)
                        {
                            _currentJumpState = JumpState.Apex;
                        }
                        break;

                    case JumpState.Apex:
                        if (verticalVelocity < 0)
                        {
                            _currentJumpState = JumpState.Falling;
                        }
                        break;
                }
            }


        }

        private void ApplyJumpPhysics(){
            switch (_currentJumpState)
            {
                case JumpState.Grounded:
                    _velocity = -2f;
                    break;

                case JumpState.JumpInitiation:
                    _velocity = jumpForce;
                    break;

                case JumpState.DoubleJumping:
                    _velocity = doubleJumpForce;
                    break;

                case JumpState.Rising:
                case JumpState.Apex:
                case JumpState.Falling:
                    _velocity += c_gravity * Time.fixedDeltaTime;
                    break;

                case JumpState.Landing:
                    _landingTimer -= Time.fixedDeltaTime;
                    _velocity = 0;
                    break;
            }

            // Apply the velocity to the controller
            //
            Vector3 move = Vector3.up * _velocity * Time.fixedDeltaTime;
            controller.Move(move);

            // Clamp fall speed
            //
            _velocity = Mathf.Max(_velocity, -20f);


        }

        private void HandleStateEffects(){
            switch (_currentJumpState){
                case JumpState.JumpInitiation:
                    PlayJumpEffects();
                    break;

                case JumpState.Falling when _velocity < fastFallThreshold:
                    PlayFastFallEffects();
                    break;

                case JumpState.Landing:
                    PlayLandingEffects();
                    break;
            }
        }

        public void Jump(InputAction.CallbackContext context){
            if (context.performed)
            {
                if (controller.isGrounded)
                {
                    _currentJumpState = JumpState.JumpInitiation;
                    _isJumpButtonHeld = true;
                }
                else if (!_hasDoubleJumped)
                {
                    _currentJumpState = JumpState.DoubleJumping;
                    _hasDoubleJumped = true;
                    _isJumpButtonHeld = true;
                }
            }
            else if (context.canceled)
            {
                _isJumpButtonHeld = false;
            }


        }

        // Additional helper functions for effects
        private void PlayJumpEffects(){
            /* ... */
        }

        private void PlayFastFallEffects(){
            /* ... */
        }

        private void PlayLandingEffects(){
            /* ... */
        }
    }
}