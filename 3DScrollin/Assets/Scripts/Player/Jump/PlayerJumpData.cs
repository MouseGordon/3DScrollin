using UnityEngine;

namespace Player.Jump{
    [CreateAssetMenu(fileName = "PlayerJumpData", menuName = "Scriptable Objects/PlayerData/PlayerJumpData")]
    public class PlayerJumpData:ScriptableObject,IJumpData{
        public float JumpForce => _jumpForce;

        public float DoubleJumpForce => _doubleJumpForce;

        public float Gravity => _gravity;

        public float GroundedGravity => _groundedGravity;

        public float TerminalVelocity => _terminalVelocity;

        public float ApexThreshold => _apexThreshold;
        
        public bool EnableDoubleJump => _enableDoubleJump;
        
        [SerializeField]private float _jumpForce;
        [SerializeField]private float _doubleJumpForce;
        [SerializeField]private float _gravity;
        [SerializeField]private float _groundedGravity;
        [SerializeField]private float _terminalVelocity;
        [SerializeField]private float _apexThreshold;
        [SerializeField]private bool _enableDoubleJump;
    }
}