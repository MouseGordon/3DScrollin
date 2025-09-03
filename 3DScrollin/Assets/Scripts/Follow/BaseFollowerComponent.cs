using GameEvent;
using GameStateSystems;
using UnityEngine;

namespace Follow{
    public class BaseFollowerComponent:MonoBehaviour,IFollowConfiguration{
        [SerializeField] protected TargetMovedGameEvent targetMovedGameEvent;
        [SerializeField, Range(.1f, 2f)] private float smoothTime = 0.3F;
        [SerializeField] private Vector3 offset;

        protected IFollow follower;

        public Transform Target{ get; set;}
        
        public Vector3 Offset => offset;
        public float SmoothTime => smoothTime;
        protected bool _isInitialized;

        protected virtual void Awake(){
            _isInitialized = false;
            follower = new Follower(this);
            targetMovedGameEvent.EventAction += OnTargetMoved;
            GameManager.OnGameObjectsInitialize += Initialize;
        }

        protected virtual void Initialize(){
            _isInitialized = true;
        }
        protected virtual void OnDestroy()
        {
            targetMovedGameEvent.EventAction -= OnTargetMoved;
            GameManager.OnGameObjectsInitialize -= Initialize;
        }

        protected virtual void FixedUpdate()
        {
            if (!_isInitialized){
                return;
            }
            if (Target == null){
                return;
            }
            transform.position = follower.UpdatePosition(transform.position, Target.position);
        }

        protected virtual void OnTargetMoved(Transform target){
            Target = target;
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (targetMovedGameEvent == null)
            {
                Debug.LogWarning("TargetMovedGameEvent not assigned!");
            }
        }
#endif

    }
}