using GameStateSystems;
using SaveSystems;

namespace SavableComponent{
    public interface ISaveableComponent<T> where T : class, ISaveData{
        void Initialize();
        //void OnSaveRequested();
        void SetSaveEventChannel(ISaveEventChannel saveEventChannel);
        T GetSaveData();
        bool IsInitialized { get; }
    }
}
