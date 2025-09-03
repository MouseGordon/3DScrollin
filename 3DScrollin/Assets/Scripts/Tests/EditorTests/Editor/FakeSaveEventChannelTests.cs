using System;
using System.Collections.Generic;
using System.Linq;
using GameStateSystems;
using SaveSystems;

namespace Tests.TestDoubles
{
    public class FakeSaveEventChannel : ISaveEventChannel
    {
        public event Action OnSaveRequested;
        public event Action<bool> OnSaveCompleted;
        public event Action OnLoadCompleted;

        // Test helpers
        public List<ISaveData> RegisteredSaveData { get; } = new List<ISaveData>();
        public List<ISaveData> UnregisteredSaveData { get; } = new List<ISaveData>();
        public bool IsSubscribedToSaveRequested => OnSaveRequested != null;
        public int SaveRequestedCallCount { get; private set; }
        
        private readonly Dictionary<Guid, ISaveData> _storedData = new Dictionary<Guid, ISaveData>();
        private readonly Dictionary<Type, List<ISaveData>> _storedDataByType = new Dictionary<Type, List<ISaveData>>();

        public void RequestSave()
        {
            SaveRequestedCallCount++;
            OnSaveRequested?.Invoke();
        }

        public void TriggerLoadCompleted()
        {
            OnLoadCompleted?.Invoke();
        }

        public void RegisterSaveData(ISaveData data)
        {
            RegisteredSaveData.Add(data);
            _storedData[data.Id] = data;
            
            var type = data.GetType();
            if (!_storedDataByType.ContainsKey(type))
                _storedDataByType[type] = new List<ISaveData>();
            _storedDataByType[type].Add(data);
        }

        public void UnregisterSaveData(ISaveData data)
        {
            UnregisteredSaveData.Add(data);
            RegisteredSaveData.Remove(data);
            _storedData.Remove(data.Id);
            
            var type = data.GetType();
            if (_storedDataByType.ContainsKey(type))
                _storedDataByType[type].Remove(data);
        }

        public T LoadData<T>(Guid id) where T : class, ISaveData
        {
            return _storedData.TryGetValue(id, out var data) ? data as T : null;
        }

        public T LoadDataByType<T>() where T : class, ISaveData
        {
            var type = typeof(T);
            return _storedDataByType.ContainsKey(type) && _storedDataByType[type].Any() 
                ? _storedDataByType[type].First() as T 
                : null;
        }

        public IEnumerable<T> LoadAllDataByType<T>() where T : class, ISaveData
        {
            var type = typeof(T);
            return _storedDataByType.ContainsKey(type) 
                ? _storedDataByType[type].Cast<T>() 
                : Enumerable.Empty<T>();
        }

        public bool TryLoadData<T>(Guid id, out T data) where T : class, ISaveData
        {
            data = LoadData<T>(id);
            return data != null;
        }

        // Test helper methods
        public void SetupLoadData<T>(T data) where T : class, ISaveData
        {
            _storedData[data.Id] = data;
            
            var type = typeof(T);
            if (!_storedDataByType.ContainsKey(type))
                _storedDataByType[type] = new List<ISaveData>();
            _storedDataByType[type].Add(data);
        }

        public void TriggerSaveRequested()
        {
            OnSaveRequested?.Invoke();
        }

        public void TriggerSaveCompleted(bool success)
        {
            OnSaveCompleted?.Invoke(success);
        }

        public void Clear()
        {
            RegisteredSaveData.Clear();
            UnregisteredSaveData.Clear();
            _storedData.Clear();
            _storedDataByType.Clear();
            SaveRequestedCallCount = 0;
            OnSaveRequested = null;
            OnSaveCompleted = null;
            OnLoadCompleted = null;
        }
    }
}
