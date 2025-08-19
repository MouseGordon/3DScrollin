using UnityEngine;

namespace Follow{
    public interface IFollowConfiguration

    {
        Transform Target { get; set; }
        Vector3 Offset { get; }
        float SmoothTime { get; }
    }
}
