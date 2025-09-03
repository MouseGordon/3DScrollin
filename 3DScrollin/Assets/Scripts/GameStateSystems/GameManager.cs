using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace GameStateSystems{
    public class GameManager{
        private static GameManager Instance{ get; set; }

        // The one event all game objects should listen to
        public static event Action OnGameObjectsInitialize;

        private SaveEventChannel _saveEventChannel; 
        private LoadingStateChannel _loadingStateChannel;
        private InputSystem_Actions inputActions;

        private InputAction rewind;

        public GameManager(SaveEventChannel saveEventChannel, LoadingStateChannel loadingStateChannel){
            Debug.Log("GameManager initializing...");
            _saveEventChannel = saveEventChannel;
            _loadingStateChannel = loadingStateChannel; 
            Instance = this;
            
            InitializeInput();
            Debug.Log("✓ GameManager ready");
            
        }
        private void InitializeInput(){
            inputActions = new InputSystem_Actions();
            rewind = inputActions.Player.Rewind;
            rewind.Enable();
            rewind.performed += TriggerSave;
        }

        private void TriggerSave(InputAction.CallbackContext callbackContext){
            Debug.Log("Save requested via input...");
            _saveEventChannel?.RequestSave();
        }
        

        public IEnumerator LoadScene(string sceneName){
            Debug.Log($"Loading scene: {sceneName}");

            var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            while (asyncLoad != null && !asyncLoad.isDone){
                yield return null;
            }

            Debug.Log("✓ Scene loaded");

            // Wait one frame for all Awake/Start calls to complete
            yield return null;

            // Now notify all game objects they can initialize
            Debug.Log("🚀 Notifying game objects to initialize...");
            OnGameObjectsInitialize?.Invoke();

            // Wait another frame for all objects to finish loading their data
            yield return null;

            // Now notify that the entire load process is complete
            Debug.Log("✓ Load process complete");
            _saveEventChannel?.TriggerLoadCompleted();
            yield return null;
            // Signal that loading is complete
            _loadingStateChannel?.CompleteLoading();
        }


        // Helper method for manual scene transitions
        public void LoadSceneAndNotify(string sceneName, MonoBehaviour coroutineHandler){
            coroutineHandler.StartCoroutine(LoadScene(sceneName));
        }

        private void OnDestroy(){
            rewind.performed -= TriggerSave;
        }
    }
}