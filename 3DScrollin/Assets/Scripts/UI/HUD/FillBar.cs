using Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.HUD{
    public class FillBar : MonoBehaviour{
        enum FillType{
            Bar, // Uses width scaling
            Radial // Uses scale transformation
        }

        [SerializeField] private FillType fillType;
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private string fillElementName = "RadialFill";
        [SerializeField] private CoreCharacterData coreCharacterData;

        private VisualElement _fillElement;

        private void Awake(){
            _fillElement = uiDocument.rootVisualElement.Q<VisualElement>(fillElementName);

            if (_fillElement == null){
                Debug.LogError($"Fill element '{fillElementName}' not found!");
                return;
            }

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
                _fillElement.style.width = Length.Percent(fillAmount * 100);
                return;
            }

            // Radial
            // 
            if (fillAmount <= 0.01f) // Hide completely when nearly empty
            {
                _fillElement.style.opacity = 0;
                _fillElement.style.scale = new StyleScale(Vector2.zero);
            }
            else{
                _fillElement.style.opacity = 1;
                _fillElement.style.scale = new StyleScale(new Vector2(fillAmount, fillAmount));
            }


        }
    }
}