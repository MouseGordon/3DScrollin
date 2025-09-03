using GameStateSystems;
using SaveSystems;
using UnityEngine;

namespace SavableComponent{
    [System.Serializable]
    public abstract class SaveableComponentBase<T> : MonoBehaviour, ISaveableComponent<T> where T : class, ISaveData{
        [SerializeField] protected SaveEventChannel eventChannel;

        private SaveableSystem<T> _saveableSystem;
        private ISaveEventChannel _saveEventChannelInterface;

        // Public properties for interface compliance
        public T GetSaveData() => _saveableSystem?.SaveData;
        public bool IsInitialized => _saveableSystem?.IsInitialized ?? false;

        protected virtual void Awake(){
            _saveEventChannelInterface = eventChannel;
            SubscribeToInitialization();
        }
        

        protected void SubscribeToInitialization(){
            GameManager.OnGameObjectsInitialize += Initialize;
        }

        protected void UnsubscribeFromInitialization(){
            GameManager.OnGameObjectsInitialize -= Initialize;
        }

        // Abstract methods for subclasses (unchanged)
        protected abstract T CreateSaveData();
        protected abstract void OnDataLoaded(T loadedData);
        protected abstract void UpdateSaveData();

        // Delegate to handler
        public virtual void Initialize(){
            _saveableSystem = new SaveableSystem<T>(
                _saveEventChannelInterface,
                CreateSaveData,
                OnDataLoaded,
                UpdateSaveData
            );
        }

        // For dependency injection in tests
        public virtual void SetSaveEventChannel(ISaveEventChannel saveEventChannel){
            _saveEventChannelInterface = saveEventChannel;

            // Recreate handler with new channel
            _saveableSystem?.Dispose();
            _saveableSystem = new SaveableSystem<T>(
                _saveEventChannelInterface,
                CreateSaveData,
                OnDataLoaded,
                UpdateSaveData
            );
        }

        protected virtual void OnDestroy(){
            _saveableSystem?.Dispose();
            UnsubscribeFromInitialization();
        }
        
    }
} 