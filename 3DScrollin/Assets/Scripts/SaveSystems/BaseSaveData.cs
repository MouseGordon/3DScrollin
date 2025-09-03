using System;

namespace SaveSystems{
    [Serializable]
    public abstract class BaseSaveData: ISaveData{
        public DateTime SaveTimeUtc { get; private set; }
        protected SaveSystem SaveSystem;

        public abstract Guid Id { get; }

        protected BaseSaveData()
        {
            SaveTimeUtc = DateTime.UtcNow;
        }
    }

}