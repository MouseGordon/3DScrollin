using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Jump{
    public class JumpSystem{
        public float Velocity => _velocity;
        public JumpState CurrentJumpState => _currentJumpState;

        public enum JumpState{
            Grounded,
            JumpInitiation,
            Rising,
            Apex,
            Falling,
            Landing,
            DoubleJumping
        }

        private readonly IJumpData _jumpData;

        private JumpState _currentJumpState = JumpState.Grounded;
        private float _velocity;
        private float _landingTimer = 0;
        private bool _isJumpButtonHeld;
        private bool _hasDoubleJumped;
        private bool _isGrounded;
        private float _gravity;
        private float _groundedGravity;

        public JumpSystem(IJumpData jumpData, float gravity, float groundedGraivty){
            _jumpData = jumpData;
            _gravity = gravity;
            _groundedGravity = groundedGraivty;
        }

        public Vector3 CalculateJumpVelocity(bool isGrounded)
        {
            _isGrounded = isGrounded;
    
            // First apply physics based on current state
            Vector3 movement = ApplyJumpPhysics();
            
            // Then update state for next frame
            UpdateJumpState();
            
            return movement;
        }


        public void HandleJumpInput(InputAction.CallbackContext context){
           StartJump(context.performed);
        }

        public void StartJump(bool performed){
            if (performed){
                if (_isGrounded){
                    InitiateJump();
                }
                else if (!_hasDoubleJumped &&
                         _jumpData.EnableDoubleJump){
                    InitiateDoubleJump();
                }

                return;
            }

            _isJumpButtonHeld = false;
        }

        private void InitiateJump(){
            _currentJumpState = JumpState.JumpInitiation;
            _isJumpButtonHeld = true;
        }

        private void InitiateDoubleJump(){
            _currentJumpState = JumpState.DoubleJumping;
            _hasDoubleJumped = true;
            _isJumpButtonHeld = true;
        }

        private void UpdateJumpState()
        {
            switch (_currentJumpState)
            {
                case JumpState.JumpInitiation:
                    if (_velocity > 0)
                    {
                        _currentJumpState = JumpState.Rising;
                    }
                    break;

                case JumpState.DoubleJumping:
                    // Remove the immediate transition to Rising
                    // Let ApplyJumpPhysics() handle the velocity first
                    _currentJumpState = JumpState.Rising;
                    break;

                case JumpState.Rising:
                    if (_velocity <= _jumpData.ApexThreshold-2)
                    {
                        _currentJumpState = JumpState.Apex;
                    }
                    break;

                case JumpState.Apex:
                    if (_velocity < _jumpData.ApexThreshold)
                    {
                        _currentJumpState = JumpState.Falling;
                    }
                    break;

                case JumpState.Falling:
                    if (_isGrounded)  // Only detect ground when actually moving downward
                    {
                        _currentJumpState = JumpState.Landing;
                    }
                    break;

                case JumpState.Landing:
                    _currentJumpState = JumpState.Grounded;
                    break;
            }
        }


        private Vector3 ApplyJumpPhysics(){
            
            switch (_currentJumpState){
                case JumpState.Grounded:
                    _velocity = _groundedGravity;
                    _hasDoubleJumped = false;
                    break;

                case JumpState.JumpInitiation:
                    _velocity = _jumpData.JumpForce;
                    break;

                case JumpState.DoubleJumping:
                    _velocity = _jumpData.DoubleJumpForce;
                    break;

                case JumpState.Rising:
                case JumpState.Apex:
                    _velocity += _gravity * Time.fixedDeltaTime;
                    break;
                case JumpState.Falling:
                    _velocity += (_gravity*3f) * Time.fixedDeltaTime;
                     break;

                case JumpState.Landing:
                    _landingTimer -= Time.fixedDeltaTime;
                    _velocity = 0;
                    break;
            }

            _velocity = Mathf.Max(_velocity, _jumpData.TerminalVelocity);
            return Vector3.up * (_velocity * Time.fixedDeltaTime);
        }

        /*private void HandleStateEffects(){
            if (_effectPlayer == null) return;

            switch (_currentJumpState){
                case JumpState.JumpInitiation:
                    _effectPlayer.PlayJumpEffect();
                    break;
                case JumpState.Falling when _velocity < _jumpData.FastFallThreshold:
                    _effectPlayer.PlayFastFallEffect();
                    break;
                case JumpState.Landing:
                    _effectPlayer.PlayLandingEffect();
                    break;
            }
        }*/
    }

}
