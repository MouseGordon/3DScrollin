using UnityEngine;

namespace Player.Movement{
    public class MovementSystem
    {
        private Vector2 _moveDirection;
        private float _currentSpeed;
        private bool _sprintToggled;
    
        private readonly float _tiredSpeed;
        private readonly float _movementSpeed;
        private readonly float _sprintSpeed;

        public bool IsSprinting => _sprintToggled && _moveDirection.x != 0;
    
        public MovementSystem(IMovementData iMovementData){
            if (iMovementData == null){
                throw new System.ArgumentNullException(nameof(iMovementData));   
            }
            IMovementData movementData = iMovementData;

            _tiredSpeed = movementData.TiredSpeed;
            _movementSpeed = movementData.MovementSpeed;
            _sprintSpeed = movementData.SprintSpeed;
            _currentSpeed = _movementSpeed;
        }
    
        public void SetMoveDirection(Vector2 direction)
        {
            _moveDirection = direction;
        }
    
        public void ToggleSprint()
        {
            if (Mathf.Approximately(_currentSpeed, _sprintSpeed))
            {
                _currentSpeed = _movementSpeed;
                _sprintToggled = false;
                return;
            }

            _sprintToggled = true;
            _currentSpeed = _sprintSpeed;
        }
    
        public Vector3 Move(float verticalVelocity, bool exhausted = false)
        {
            float horizontalSpeed = exhausted ? _tiredSpeed : _currentSpeed;
    
            return new Vector3(
                _moveDirection.x * horizontalSpeed * Time.fixedDeltaTime,
                verticalVelocity,  // vertical velocity already includes Time.fixedDeltaTime from physics calculations
                0
            );
        }

    }
}