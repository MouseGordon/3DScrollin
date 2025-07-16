using System;
using System.Collections;
using GameEvent;
using UnityEngine;

namespace HUD{
    public class UIBlink : MonoBehaviour{
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] private CoolDownGameEvent coolDownEvent;
        [SerializeField] private CoolDownGameEvent blinkEvent;

        private bool _isBlinking = false;

        private void Awake(){
            coolDownEvent.EventAction += IsBlink;
            blinkEvent.EventAction += Blink;
        }

        private void IsBlink(bool blink){
            // cooldown event returns true on complete.
            //
            _isBlinking = !blink;
            if (!_isBlinking){
                canvasGroup.alpha = 1;
                return;
            }

            blinkEvent.StartCoolDown(this);
        }

        private void Blink(bool blink){
            if (!_isBlinking){
                return;
            }
            
            if (blink){
                blinkEvent.StartCoolDown(this);
                return;
            }
            
            canvasGroup.alpha = canvasGroup.alpha > 0 ? 0:1;
       }
       
    }
}