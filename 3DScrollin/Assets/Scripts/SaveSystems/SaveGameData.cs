using System;
using System.Collections.Generic;

namespace SaveSystems{

    // Container for all save data
    [Serializable]
    public class SaveGameData{
        public string SaveTimeUtc;
        public string GameVersion;
        public List<ISaveData> SavedObjects = new List<ISaveData>();
    }

}
