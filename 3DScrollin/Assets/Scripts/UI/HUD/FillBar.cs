using Player;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace UI.HUD{
    public class FillBar:MonoBehaviour{
        enum FillType{
            Bar,
            Fill
        }
        
        [SerializeField] private FillType fillType;
        [SerializeField] private RectTransform fillTransform;
        [SerializeField] private Image fillImage;
        [SerializeField] private CoreCharacterData coreCharacterData;

        private void Awake(){
            if (coreCharacterData == null){
                return;
            }
            coreCharacterData.StaminaData.StaminaChangedActionEvent += UpdateFillBar;
        }

        private void OnDestroy(){
            if (coreCharacterData == null){
                return;
            } 
            coreCharacterData.StaminaData.StaminaChangedActionEvent -= UpdateFillBar;
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