using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace SaveSystems{
    // Pure C# save system - handles the global save/load logic
    public class SaveSystem{
        public event Action<bool> OnSaveCompleted;

        private readonly Dictionary<Guid, ISaveData> _pendingSaveData = new();

        public void RegisterSaveData(ISaveData data){
            if (data == null){
                Debug.LogError("Attempted to register null save data");
                return;
            }

            if (_pendingSaveData.TryAdd(data.Id, data)){
                return;
            }

            _pendingSaveData[data.Id] = data;
        }

        public void UnregisterSaveData(ISaveData data){
            if (data == null){
                Debug.LogError("Attempted to unregister null save data");
                return;
            }

            _pendingSaveData.Remove(data.Id);
        }

        // Type-safe loading methods
        public T LoadData<T>(Guid id) where T : class, ISaveData{
            return TryGetSaveData(id) as T;
        }

        public T LoadDataByType<T>() where T : class, ISaveData{
            var savedObjects = LoadFromFile()?.SavedObjects;
            return savedObjects?.OfType<T>().FirstOrDefault();
        }

        public IEnumerable<T> LoadAllDataByType<T>() where T : class, ISaveData{
            var savedObjects = LoadFromFile()?.SavedObjects;
            return savedObjects?.OfType<T>() ?? Enumerable.Empty<T>();
        }

        public bool TryLoadData<T>(Guid id, out T data) where T : class, ISaveData{
            data = TryGetSaveData(id) as T;
            return data != null;
        }

        public void SaveToFile(){
            try{
                if (_pendingSaveData.Count == 0){
                    Debug.LogWarning("No save data registered before saving.");
                    OnSaveCompleted?.Invoke(true);
                    return;
                }

                var saveData = new SaveGameData {
                    SaveTimeUtc = DateTime.UtcNow.ToShortTimeString(),
                    GameVersion = Application.version,
                    SavedObjects = _pendingSaveData.Values.ToList()
                };

                var settings = new JsonSerializerSettings {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                };

                string json = JsonConvert.SerializeObject(saveData, settings);
                string savePath = Path.Combine(Application.persistentDataPath, "save.json");
                File.WriteAllText(savePath, json);

                Debug.Log($"Successfully saved {_pendingSaveData.Count} objects to {savePath}");
                OnSaveCompleted?.Invoke(true);
            }
            catch (Exception e){
                Debug.LogError($"Failed to save game: {e.Message}");
                OnSaveCompleted?.Invoke(false);
            }
            finally{
                _pendingSaveData.Clear();
            }
        }

        private SaveGameData LoadFromFile(){
            try{
                string savePath = Path.Combine(Application.persistentDataPath, "save.json");
                if (!File.Exists(savePath)){
                    Debug.LogWarning("No save file found");
                    return null;
                }

                string json = File.ReadAllText(savePath);
                var settings = new JsonSerializerSettings {
                    TypeNameHandling = TypeNameHandling.Auto
                };

                return JsonConvert.DeserializeObject<SaveGameData>(json, settings);
            }
            catch (Exception e){
                Debug.LogError($"Failed to load game: {e.Message}");
                return null;
            }
        }

        public ISaveData TryGetSaveData(Guid id){
            var savedObjects = LoadFromFile()?.SavedObjects;
            return savedObjects?.FirstOrDefault(x => x.Id == id);
        }

        public void ClearForTesting(){
            _pendingSaveData.Clear();
        }
    }
}