using System.Collections;
using GameStateSystems;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.LoadingUI
{
    [RequireComponent(typeof(UIDocument))]
    public class LoadingScreenComponent : MonoBehaviour{
[SerializeField] private LoadingStateChannel loadingStateChannel;

        private UIDocument _uiDocument;
        private VisualElement _loadingScreen;

        private void Awake(){
            _uiDocument = GetComponent<UIDocument>();
            _loadingScreen = _uiDocument.rootVisualElement.Q<VisualElement>("Container");
            loadingStateChannel.OnLoadingCompleted += HideLoadingScreen;
        }

        private void OnDestroy(){
            loadingStateChannel.OnLoadingCompleted -= HideLoadingScreen;
        }

        private void HideLoadingScreen(){
            _loadingScreen.AddToClassList("hidden");
        }

        private void OnValidate(){
            if (loadingStateChannel == null){
                Debug.LogError($"LoadingStateChannel is null on {name}");
            }
        }

    }
}