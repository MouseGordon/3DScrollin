using UnityEngine;

namespace Player.Gravity{
        
    [CreateAssetMenu(fileName = "PlayerGravityData", menuName = "Scriptable Objects/PlayerData/PlayerGravityData")]
    public class PlayerGravityData:ScriptableObject,IGravityData{
        public float GravityForce => _gravityForce;
        public float TerminalVelocity => _terminalVelocity;
        public float GroundedGravity => _groundedGravity;

        [SerializeField]private float _gravityForce;
        [SerializeField]private float _terminalVelocity;
        [SerializeField]private float _groundedGravity;
    }
}