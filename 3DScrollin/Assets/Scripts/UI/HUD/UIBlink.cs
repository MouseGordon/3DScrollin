using GameEvent;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.HUD{
    public class UIBlink : MonoBehaviour{
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private string blinkElementName = "RadialBackground";
        [SerializeField] private CoolDownGameEvent coolDownEvent;
        [SerializeField] private CoolDownGameEvent blinkEvent;

        private VisualElement _blinkElement;
        private bool _isBlinking = false;

        private void Awake(){
            _blinkElement = uiDocument.rootVisualElement.Q<VisualElement>(blinkElementName);

            if (_blinkElement == null){
                Debug.LogError($"Blink element '{blinkElementName}' not found!");
                return;
            }

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
            float currentOpacity = _blinkElement.style.opacity.value;
            _blinkElement.style.opacity = currentOpacity == 1 ? 0 : 1;
        }

        private void BlinkDurationComplete(bool blink){
            _isBlinking = false;
            _blinkElement.style.opacity = 1;
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