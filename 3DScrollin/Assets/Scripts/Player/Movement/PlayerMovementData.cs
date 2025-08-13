using UnityEngine;

namespace Player.Movement{
    [CreateAssetMenu(fileName = "PlayerMovementData", menuName = "Scriptable Objects/PlayerMovementData")]
    public class PlayerMovementData:ScriptableObject,IMovementData{
        public float SprintSpeed => _sprintSpeed;
        public float MovementSpeed => _movementSpeed;
        public float TiredSpeed => _tiredSpeed;
        
        [SerializeField] private float _sprintSpeed;
        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _tiredSpeed;
    }
}