using System;
using System.Collections;
using GameEvent;
using UnityEngine;

namespace HUD{
    public class UIBlink : MonoBehaviour{
        [SerializeField] CanvasGroup canvasGroup;
        // the total time we should be blinking.
        //
        [SerializeField] private CoolDownGameEvent coolDownEvent;
        [SerializeField] private CoolDownGameEvent blinkEvent;

        private bool _isBlinking = false;

        private void Awake(){
            coolDownEvent.StartCoolDownAction += StartBlinkDuration;
            coolDownEvent.EventAction += BlinkDurationComplete;
            
            blinkEvent.StartCoolDownAction += StartBlink;
            blinkEvent.EventAction += Blink;
        }

        private void StartBlinkDuration(){
            _isBlinking = true;
            blinkEvent.StartCoolDown(this);
        }

        private void StartBlink(){
            canvasGroup.alpha = canvasGroup.alpha == 1 ? 0 : 1;;
        }
        
        private void BlinkDurationComplete(bool blink){
            _isBlinking = false;
            canvasGroup.alpha = 1;
        }

        private void Blink(bool blink = false){
            if (!_isBlinking){
                return;
            }
            
            blinkEvent.StartCoolDown(this);
       }

        private void OnDestroy(){
            coolDownEvent.StartCoolDownAction -= StartBlinkDuration;
            coolDownEvent.EventAction -= BlinkDurationComplete;
            
            blinkEvent.StartCoolDownAction -= StartBlink;
            blinkEvent.EventAction -= Blink;
        }
    }
}