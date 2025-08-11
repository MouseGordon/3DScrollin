using System;
using System.Collections;
using UnityEngine;

namespace GameEvent{
    [CreateAssetMenu(fileName = "CoolDownGameEvent", menuName = "Scriptable Objects/GameEvent/CoolDownGameEvent")]
    public class CoolDownGameEvent : GameEvent<bool>{
        public float CoolDownDuration{
            get => coolDownDuration;
            set => coolDownDuration = value;
        }
        public bool IsInCooldown => _remainingTime > 0;
        public Action StartCoolDownAction;
        
        [SerializeField] private float coolDownDuration = 3;
       
        private float _remainingTime;
        private bool _isPaused;
        private Coroutine _currentCooldown;
        private MonoBehaviour _runner;
        
        public void StartCoolDown(MonoBehaviour monoBehaviour){
            if (monoBehaviour == null || IsInCooldown){
                return;
            }
            
            if (_currentCooldown != null){
                monoBehaviour.StopCoroutine(_currentCooldown);
            }

            _runner = monoBehaviour;
            _isPaused = false;
            StartCoolDownAction?.Invoke();
            _currentCooldown = monoBehaviour.StartCoroutine(CoolDown());
        }

        public void PauseCooldown(){
            _isPaused = true;
        }

        public void ResumeCooldown(){
            _isPaused = false;
        }

        public void CancelCooldown(){
            if (_currentCooldown != null &&
                _runner != null){
                _runner.StopCoroutine(_currentCooldown);
                _currentCooldown = null;
                _remainingTime = 0;
                Raise(false);
            }
        }
        
        private void OnEnable(){
            _isPaused = false;
            _remainingTime = 0;
        }

        private IEnumerator CoolDown(){
            _remainingTime = coolDownDuration;
            float endTime = Time.time + coolDownDuration;

            while (_remainingTime > 0){
                if (_isPaused){
                    // When paused, adjust the end time to maintain the correct remaining duration
                    //
                    endTime = Time.time + _remainingTime;
                    yield return null;
                    continue;
                }

                _remainingTime = endTime - Time.time;
                yield return null;
            }

            _currentCooldown = null;
            _remainingTime = 0;
            Raise(true);
        }
        
    }
    
}