using GameEvent;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace HUD{
    public class FillBar:MonoBehaviour{
        enum FillType{
            Bar,
            Fill
        }
        
        [SerializeField] private FillType fillType;
        [SerializeField] private RectTransform fillTransform;
        [SerializeField] private Image fillImage;
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

        private void UpdateFillBar(float fillAmount){
            if (fillType == FillType.Bar){
                fillTransform.localScale = new Vector3(fillAmount, fillTransform.localScale.y, fillTransform.localScale.z);
                return;
            }
            fillImage.fillAmount = fillAmount;
        }
    }
}