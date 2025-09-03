// GameState.cs

using System;
using UnityEngine;

namespace GameStateSystems{
    [CreateAssetMenu(fileName = "GameStateData", menuName = "Game/GameStateData")]
    public class GameStateData : ScriptableObject
    {
        public bool IsInitialized { get; private set; }
    
        public bool IsNewGame => !HasExistingSave();
        public bool IsPaused { get; private set; }
    
        // Save data
        public Vector3 LastPlayerPosition;
        public string LastCheckpointId;
        // Add other persistent data as needed
    
        public event Action<bool> OnPauseStateChanged;
    
    
        public void Initialize()
        {
            if (!IsInitialized)
            {
                // Load any saved data
                LoadSavedData();
                IsInitialized = true;
            }
        }
    
        private void LoadSavedData()
        {
            // Load any persistent data
        }

        public void TogglePause()
        {
            IsPaused = !IsPaused;
            OnPauseStateChanged?.Invoke(IsPaused);
        }
    
        private bool HasExistingSave()
        {
            // Check if save data exists
            return PlayerPrefs.HasKey("SaveExists");
        }
    
        public void StartNewGame()
        {
            LastPlayerPosition = Vector3.zero; // Or your default spawn point
            LastCheckpointId = "StartingPoint";
            PlayerPrefs.SetInt("SaveExists", 1);
            PlayerPrefs.Save();
        }
    }
}