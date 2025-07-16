using System;
using System.Collections;
using UnityEngine;

namespace GameEvent{
    [CreateAssetMenu(fileName = "CoolDownGameEvent", menuName = "Scriptable Objects/GameEvent/CoolDownGameEvent")]
    public class CoolDownGameEvent : GameEvent<bool>{
        [SerializeField] private float coolDownDuration = 3;

        private WaitForSeconds _waitForSeconds;

        private void OnEnable(){
           _waitForSeconds = new WaitForSeconds(coolDownDuration);
        }
        private IEnumerator CoolDown(){
            yield return _waitForSeconds;
            Raise(true);
        }

        public void StartCoolDown(MonoBehaviour monoBehaviour){
            Raise(false);
            monoBehaviour.StartCoroutine(CoolDown());
        }
    }
}