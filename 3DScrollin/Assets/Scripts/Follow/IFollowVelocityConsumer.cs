using UnityEngine;

namespace Follow{
    public interface IFollowVelocityConsumer
    {
        void OnVelocityUpdated(Vector3 velocity);
    }
}