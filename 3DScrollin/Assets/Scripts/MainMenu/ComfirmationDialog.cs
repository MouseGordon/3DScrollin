using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MainMenu{
    public class ConfirmationDialog : MonoBehaviour{
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        [Header("Animation")] [SerializeField] private UnityEvent onShow;
        [SerializeField] private UnityEvent onHide;

        public void Show(string message, UnityAction onConfirm, UnityAction onCancel){
            messageText.text = message;

            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();

            confirmButton.onClick.AddListener(onConfirm);
            cancelButton.onClick.AddListener(onCancel);

            gameObject.SetActive(true);
            onShow?.Invoke();
        }
    }

}
