using System;
using GameStateSystems;
using SaveSystems;

namespace SavableComponent
{
    public class SaveableSystem<T> : IDisposable where T : class, ISaveData
    {
        private readonly ISaveEventChannel _saveEventChannel;
        private readonly Func<T> _createSaveDataFunc;
        private readonly Action<T> _onDataLoadedAction;
        private readonly Action _updateSaveDataAction;
        
        private T _saveData;
        private bool _isInitialized;

        public T SaveData => _saveData;
        public bool IsInitialized => _isInitialized;

        public SaveableSystem(
            ISaveEventChannel saveEventChannel,
            Func<T> createSaveData,
            Action<T> onDataLoaded,
            Action updateSaveData)
        {
            _saveEventChannel = saveEventChannel ?? throw new ArgumentNullException(nameof(saveEventChannel));
            _createSaveDataFunc = createSaveData ?? throw new ArgumentNullException(nameof(createSaveData));
            _onDataLoadedAction = onDataLoaded ?? throw new ArgumentNullException(nameof(onDataLoaded));
            _updateSaveDataAction = updateSaveData ?? throw new ArgumentNullException(nameof(updateSaveData));

            _saveData = _createSaveDataFunc();

            LoadExistingData();
            
            _isInitialized = true;
            
            // Subscribe to save event
            //
            _saveEventChannel.OnSaveRequested += OnSaveRequested;
        }
        
        public void LoadExistingData()
        {
            if (_saveData == null){
                return;
            }

            var loadedData = _saveEventChannel.LoadData<T>(_saveData.Id);
            if (loadedData != null)
            {
                _onDataLoadedAction(loadedData);
            }
        }
        
        private void OnSaveRequested()
        {
            _updateSaveDataAction();
            _saveEventChannel.RegisterSaveData(_saveData);
        }

        public void Dispose()
        {
            // Unsubscribe from save event
            //
            _saveEventChannel.OnSaveRequested -= OnSaveRequested;
        }
    }
}
