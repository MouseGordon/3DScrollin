using System;
using GameEvent;
using UnityEngine;

namespace HUD{
    public class FillBar:MonoBehaviour{
        [SerializeField] private RectTransform fillBar;
        [SerializeField] private StaminaChangedEvent staminaEvent;

        private void Awake(){
            if (staminaEvent == null){
                return;
            }
            staminaEvent.EventAction += UpdateFillBar;
        }

        private void OnDestroy(){
            if (staminaEvent == null){
                return;
            } 
            staminaEvent.EventAction -= UpdateFillBar;
        }
        public void UpdateFillBar(float fillAmount){
            fillBar.localScale = new Vector3(fillAmount,fillBar.localScale.y,fillBar.localScale.z);
        }
    }
}