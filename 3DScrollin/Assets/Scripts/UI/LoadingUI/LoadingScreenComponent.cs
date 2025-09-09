using System.Collections;
using GameStateSystems;
using UnityEngine;

namespace UI.LoadingUI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class LoadingScreenComponent : MonoBehaviour
    {
        [SerializeField] private LoadingStateChannel loadingStateChannel;
        
        private CanvasGroup _canvasGroup;
        private float _fadeOutDuration;

        private void Awake(){
            // Make sure this persists across scene loads
            //
            DontDestroyOnLoad(gameObject);
            _canvasGroup = GetComponent<CanvasGroup>();
            _fadeOutDuration = loadingStateChannel.FadeOutDuration;
            
            loadingStateChannel.OnLoadingStarted += ShowLoadingScreen;
            loadingStateChannel.OnLoadingCompleted += HideLoadingScreen;
            
        }

        private void OnDestroy(){
            if (loadingStateChannel == null){
                return;
            }
            
            loadingStateChannel.OnLoadingStarted -= ShowLoadingScreen;
            loadingStateChannel.OnLoadingCompleted -= HideLoadingScreen;
        }

        private void ShowLoadingScreen(){
            if (_canvasGroup == null){
                return;
            }
            
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        private void HideLoadingScreen(){
            StartCoroutine(FadeOutLoadingScreen());
        }

        private IEnumerator FadeOutLoadingScreen(){
            if (_canvasGroup != null){
                float startAlpha = _canvasGroup.alpha;
                float elapsedTime = 0f;
                
                while (elapsedTime < _fadeOutDuration){
                    elapsedTime += Time.unscaledDeltaTime;
                    float alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / _fadeOutDuration);
                    _canvasGroup.alpha = alpha;
                    yield return null;
                }

                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }
            
        }

        private void OnValidate(){
            if (loadingStateChannel == null){
                Debug.LogError($"LoadingStateChannel is null on {name}");
            }
        }
    }
}