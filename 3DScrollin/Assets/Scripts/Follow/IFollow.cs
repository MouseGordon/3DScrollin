using UnityEngine;
namespace Follow{
    public interface IFollow
    {
        Vector3 Velocity { get; }
        Vector3 UpdatePosition(Vector3 pos, Vector3 targetPos);
    }
}