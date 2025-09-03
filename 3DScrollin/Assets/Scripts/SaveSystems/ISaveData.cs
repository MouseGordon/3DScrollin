using System;

namespace SaveSystems{
    public interface ISaveData{
        Guid Id{ get; }
        DateTime SaveTimeUtc{ get; }
    }
}