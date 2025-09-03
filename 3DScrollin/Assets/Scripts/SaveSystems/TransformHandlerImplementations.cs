using UnityEngine;

namespace SaveSystems{
    public interface ITransformHandler
    {
        void SetPosition(Vector3 position);
    }

    public class BasicTransformHandler : ITransformHandler
    {
        private readonly Transform _transform;
    
        public BasicTransformHandler(Transform transform) => _transform = transform;
    
        public void SetPosition(Vector3 position) => _transform.position = position;
    }

    public class CharacterControllerTransformHandler : ITransformHandler
    {
        private readonly CharacterController _controller;
        private readonly Transform _transform;
    
        public CharacterControllerTransformHandler(CharacterController controller, Transform transform)
        {
            _controller = controller;
            _transform = transform;
        }
    
        public void SetPosition(Vector3 position)
        {
            _controller.enabled = false;
            _transform.position = position;
            _controller.enabled = true;
        }
    }
}