using System;
using GameEvent;
using UnityEngine;

namespace Camera{
    public class FollowPlayer : MonoBehaviour{

        [SerializeField] private TargetMovedGameEvent targetMovedGameEvent;
        [SerializeField, Range(.1f, 2f)] private float smoothTime = 0.3F;
        [SerializeField] private Vector3 offset;

        private Vector3 _velocity = Vector3.zero;
        private Transform _targetTransform;

        private void Awake(){
            targetMovedGameEvent.EventAction += EventAction;
        }

        private void OnDestroy(){
            targetMovedGameEvent.EventAction -= EventAction;
        }

        private void FixedUpdate(){
            if (_targetTransform == null){
                Debug.LogWarning("No target set for camera to follow");
                return;
            }

            Debug.Assert(smoothTime > 0, "Smooth time should be positive");

            Vector3 targetPosition = _targetTransform.TransformPoint(new Vector3(0, 5, -10)) + offset;
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref _velocity,
                smoothTime
            );
        }

        private void EventAction(Transform targetTransform){
            _targetTransform = targetTransform;
        }

        #if UNITY_EDITOR
        private void OnValidate(){
            if (targetMovedGameEvent == null){
                Debug.LogWarning("TargetMovedGameEvent not assigned!");
            }
        }
        #endif

    }
}