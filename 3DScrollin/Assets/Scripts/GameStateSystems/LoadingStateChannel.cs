using System;
using UnityEngine;

namespace GameStateSystems{
    [CreateAssetMenu(fileName = "LoadingStateChannel", menuName = "Game/GameState/LoadingStateChannel")]
    public class LoadingStateChannel : ScriptableObject
    {
        public event Action OnLoadingStarted;
        public event Action OnLoadingCompleted;
        
        public float FadeOutDuration => fadeOutDuration;
        
        [SerializeField] private float fadeOutDuration = 0.5f;

        public void StartLoading()
        {
            Debug.Log("🔄 Loading started");
            OnLoadingStarted?.Invoke();
        }

        public void CompleteLoading()
        {
            Debug.Log("✓ Loading completed");
            OnLoadingCompleted?.Invoke();
        }
    }
    
}