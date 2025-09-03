using System.Collections;
using UnityEngine;

namespace GameStateSystems{
    public class InitializationManager : MonoBehaviour{
        [SerializeField] private SaveEventChannel saveEventChannel;
        [SerializeField] private LoadingStateChannel loadingStateChannel;
        [SerializeField] private string gameSceneName = "GameScene";

        private GameManager _gameManager;

        private void Awake(){
            DontDestroyOnLoad(gameObject);
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
    }
}