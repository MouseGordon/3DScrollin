using System.Collections;
using GameStateSystems;
using UnityEngine;

namespace UI.LoadingUI
{
    public class LoadingScreenComponent : MonoBehaviour
    {
        [SerializeField] private LoadingStateChannel loadingStateChannel;
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeOutDuration = 0.5f;

        private void Awake(){
            // Make sure this persists across scene loads
            DontDestroyOnLoad(gameObject);

            // Initially hidden until loading starts
            /*if (canvasGroup != null){
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }*/

            /*if (loadingPanel != null){
                loadingPanel.SetActive(false);
            }*/
        }

        private void OnEnable(){
            if (loadingStateChannel != null){
                loadingStateChannel.OnLoadingStarted += ShowLoadingScreen;
                loadingStateChannel.OnLoadingCompleted += HideLoadingScreen;
            }
        }

        private void OnDisable(){
            if (loadingStateChannel != null){
                loadingStateChannel.OnLoadingStarted -= ShowLoadingScreen;
                loadingStateChannel.OnLoadingCompleted -= HideLoadingScreen;
            }
        }

        private void ShowLoadingScreen(){
            if (loadingPanel != null){
                loadingPanel.SetActive(true);
            }

            if (canvasGroup != null){
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }

        private void HideLoadingScreen(){
            StartCoroutine(FadeOutLoadingScreen());
        }

        private IEnumerator FadeOutLoadingScreen(){
            if (canvasGroup != null){
                float startAlpha = canvasGroup.alpha;
                float elapsedTime = 0f;

                while (elapsedTime < fadeOutDuration){
                    elapsedTime += Time.unscaledDeltaTime;
                    float alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeOutDuration);
                    canvasGroup.alpha = alpha;
                    yield return null;
                }

                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            if (loadingPanel != null){
                loadingPanel.SetActive(false);
            }
        }
    }
}