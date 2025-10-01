using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameStateSystems{
    public class InitializationManager : MonoBehaviour{
        [SerializeField] private SaveEventChannel saveEventChannel;
        [SerializeField] private LoadingStateChannel loadingStateChannel;
        [SerializeField] private string gameSceneName = "GameScene";
        
        [SerializeField] private UIDocument uiDocument;
        
        private GameManager _gameManager;
        private Button _beginButton;

        private void Awake(){
            DontDestroyOnLoad(gameObject);
            
            _beginButton = uiDocument.rootVisualElement.Q<Button>("BeginButton");
            _beginButton.RegisterCallback<ClickEvent>(OnBeginClicked);
        }

        private void OnBeginClicked(ClickEvent evt){
            _beginButton.UnregisterCallback<ClickEvent>(OnBeginClicked);
            // Apply click effect
            _beginButton.style.backgroundColor = Color.grey;
            _beginButton.style.scale = new StyleScale(new Vector2(1, 1));

            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize(){
            Debug.Log("🎮 Starting initialization...");

            // Signal that loading has started
            loadingStateChannel?.StartLoading();

            // Create GameManager
            _gameManager = new GameManager(saveEventChannel, loadingStateChannel);

            // Load the game scene
            yield return _gameManager.LoadScene(gameSceneName);

            Debug.Log("🎉 Initialization complete!");
        }

        private void OnValidate(){
            if (saveEventChannel == null){
                Debug.LogError($"SaveEventChannel is null on {name}");
            }
            if (loadingStateChannel == null){
                Debug.LogError($"LoadingStateChannel is null on {name}");
            }
        }

        private void OnDestroy(){
            _gameManager.Dispose();
            _beginButton.UnregisterCallback<ClickEvent>(OnBeginClicked);
        }
    }
}