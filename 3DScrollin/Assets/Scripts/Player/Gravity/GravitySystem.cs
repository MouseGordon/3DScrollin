using System;
using UnityEngine;

namespace Player.Gravity{
    public class GravitySystem{
        private float _currentVerticalVelocity;
        
        private readonly float _gravityForce;
        private readonly float _terminalVelocity;
        private readonly float _groundedGravity;

        public GravitySystem(IGravityData gravityData){
            var data = gravityData ?? throw new ArgumentNullException(nameof(gravityData));

            _gravityForce = data.GravityForce;
            _terminalVelocity = data.TerminalVelocity;
            _groundedGravity = data.GroundedGravity;
        }
       
        public float CalculateGravity(bool isGrounded){
            if (isGrounded){
                _currentVerticalVelocity = _groundedGravity;
            }
            else{
                // Apply gravity
                //
                _currentVerticalVelocity += _gravityForce * Time.fixedDeltaTime;
                // Clamp to terminal velocity
                //
                _currentVerticalVelocity = Mathf.Max(_currentVerticalVelocity, _terminalVelocity);
            }

            return _currentVerticalVelocity;
        }

        public float GetCurrentVerticalVelocity(){
            return _currentVerticalVelocity;
        }
    }
}
