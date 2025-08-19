using System;
using UnityEngine;

namespace Companion{
    public interface IVelocityBasedEffect{
        ParticleSystem.MinMaxCurve UpdateEffect(Vector3 velocity);
    }
    
    public class CompanionParticleEffect : IVelocityBasedEffect{
        private readonly ICompanionData _data;

        public CompanionParticleEffect(ICompanionData data){
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public ParticleSystem.MinMaxCurve UpdateEffect(Vector3 velocity){
            float xVelocity = Mathf.Abs(velocity.x);
            float orbitalValue = 0f;

            if (xVelocity > _data.VelocityThreshold){
                orbitalValue = velocity.x > 0 ? -_data.OrbitalMultiplier : _data.OrbitalMultiplier;
            }

            return new ParticleSystem.MinMaxCurve(orbitalValue);
        }
    }

}