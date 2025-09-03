using System;
using System.Collections.Generic;
using SaveSystems;

namespace GameStateSystems{
    public interface ISaveEventChannel{
        event Action OnSaveRequested;
        event Action<bool> OnSaveCompleted;
        event Action OnLoadCompleted;
        
        void RequestSave();
        void TriggerLoadCompleted();
        void RegisterSaveData(ISaveData data);
        void UnregisterSaveData(ISaveData data);
        T LoadData<T>(System.Guid id) where T : class, ISaveData;
        T LoadDataByType<T>() where T : class, ISaveData;
        IEnumerable<T> LoadAllDataByType<T>() where T : class, ISaveData;
        bool TryLoadData<T>(System.Guid id, out T data) where T : class, ISaveData;
    }
}
