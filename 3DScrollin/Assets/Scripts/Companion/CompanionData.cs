using UnityEngine;

namespace Companion
{
    public interface ICompanionData{
        float VelocityThreshold { get; }
        float OrbitalMultiplier { get; }
    }

    [CreateAssetMenu(fileName = "ParticleEffectConfig", menuName = "Companion/CompanionData")]
    public class CompanionData : ScriptableObject,ICompanionData
    {
        [SerializeField] private float velocityThreshold = 0.5f;
        [SerializeField] private float orbitalMultiplier = 1.5f;

        public float VelocityThreshold => velocityThreshold;
        public float OrbitalMultiplier => orbitalMultiplier;
    }
}