using UnityEngine;

namespace Follow{
    public class Follower : IFollow
    {
        public Vector3 Velocity => velocity;
        
        private readonly IFollowConfiguration _followConfig;
        private Vector3 velocity;

        public Follower(IFollowConfiguration followConfig)
        {
            _followConfig = followConfig;
        }

        public Vector3 UpdatePosition(Vector3 pos, Vector3 targetPos)
        {
            return Vector3.SmoothDamp(
                pos,
                targetPos+ _followConfig.Offset,
                ref velocity,
                _followConfig.SmoothTime
            );
        }
    }
}