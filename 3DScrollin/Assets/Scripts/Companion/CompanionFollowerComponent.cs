using Follow;
using UnityEngine;

namespace Companion{
    public class CompanionFollowerComponent : BaseFollowerComponent, IFollowVelocityConsumer{
        [SerializeField] private ParticleSystem companionParticles;
        [SerializeField, Range(0.01f, 10f)] private float velocityThreshold = 0.05f;
        [SerializeField] CompanionData companionData;

        private IVelocityBasedEffect particleEffect;

        protected override void Awake(){
            base.Awake();
            if (companionParticles == null){
                companionParticles = GetComponentInChildren<ParticleSystem>();
            }

            particleEffect = new CompanionParticleEffect(
                companionData);
        }

        private void Update(){
            OnVelocityUpdated(follower.Velocity);
        }

        public void OnVelocityUpdated(Vector3 velocity){
            if (particleEffect == null){
                return;
            }
            var companionParticlesVelocityOverLifetime = companionParticles.velocityOverLifetime;
            companionParticlesVelocityOverLifetime.enabled = velocity.magnitude > velocityThreshold;
            companionParticlesVelocityOverLifetime.orbitalY = particleEffect.UpdateEffect(velocity);
        }
    }
    
}


