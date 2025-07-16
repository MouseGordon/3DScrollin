using GameEvent;
using UnityEngine;

namespace Camera{
    public class FollowPlayer : MonoBehaviour{
        [SerializeField] private PlayerMovedGameEvent playerMovedGameEventEvent;

        private void Awake(){
            playerMovedGameEventEvent.EventAction += EventAction;
        }

        private void OnDestroy(){
            playerMovedGameEventEvent.EventAction -= EventAction;
        }

        private void EventAction(Vector3 obj){
            Vector3 pos = obj.normalized;
            transform.position = pos;
        }
    }
}