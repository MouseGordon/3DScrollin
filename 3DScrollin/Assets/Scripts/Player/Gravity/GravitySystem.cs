using UnityEngine;

namespace Player.Gravity{
    public class GravitySystem{
        private float _gravityForce = -9.81f;
        private float _terminalVelocity = -20f;
        private float _groundedGravity = -2f;

        private float _currentVerticalVelocity;

        public GravitySystem(IGravityData gravityDataDataData){
            
        }
        public float CalculateGravity(bool isGrounded){
            if (isGrounded){
                _currentVerticalVelocity = _groundedGravity;
            }
            else{
                // Apply gravity
                _currentVerticalVelocity += _gravityForce * Time.fixedDeltaTime;
                // Clamp to terminal velocity
                _currentVerticalVelocity = Mathf.Max(_currentVerticalVelocity, _terminalVelocity);
            }

            return _currentVerticalVelocity;
        }

        public void SetVerticalVelocity(float velocity){
            _currentVerticalVelocity = velocity;
        }

        public float GetCurrentVerticalVelocity(){
            return _currentVerticalVelocity;
        }
    }
}
