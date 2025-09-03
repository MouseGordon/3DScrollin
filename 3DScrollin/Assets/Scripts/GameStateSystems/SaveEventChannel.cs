using System;
using System.Collections.Generic;
using System.Linq;
using SaveSystems;
using UnityEngine;

namespace GameStateSystems{
    [CreateAssetMenu(fileName = "SaveEventChannel", menuName = "Game/Events/SaveEventChannel")]
    public class SaveEventChannel : ScriptableObject, ISaveEventChannel{
        public event Action OnSaveRequested;
        public event Action<bool> OnSaveCompleted;
        public event Action OnLoadCompleted;

        private SaveSystem _globalSaveSystem;

        private void OnEnable(){
            if (_globalSaveSystem == null){
                _globalSaveSystem = new SaveSystem();
                _globalSaveSystem.OnSaveCompleted += SaveCompleted;
            }
        }

        // Implementation remains the same...
        public void RequestSave(){
            OnSaveRequested?.Invoke();
            _globalSaveSystem?.SaveToFile();
        }

        public void TriggerLoadCompleted(){
            OnLoadCompleted?.Invoke();
        }

        public void RegisterSaveData(ISaveData data){
            _globalSaveSystem?.RegisterSaveData(data);
        }

        public void UnregisterSaveData(ISaveData data){
            _globalSaveSystem?.UnregisterSaveData(data);
        }

        public T LoadData<T>(System.Guid id) where T : class, ISaveData{
            return _globalSaveSystem?.LoadData<T>(id);
        }

        public T LoadDataByType<T>() where T : class, ISaveData{
            return _globalSaveSystem?.LoadDataByType<T>();
        }

        public IEnumerable<T> LoadAllDataByType<T>() where T : class, ISaveData{
            return _globalSaveSystem?.LoadAllDataByType<T>() ?? Enumerable.Empty<T>();
        }

        public bool TryLoadData<T>(System.Guid id, out T data) where T : class, ISaveData{
            data = null;
            if (_globalSaveSystem == null) return false;
            return _globalSaveSystem.TryLoadData<T>(id, out data);
        }

        private void SaveCompleted(bool success){
            OnSaveCompleted?.Invoke(success);
        }
    }
}