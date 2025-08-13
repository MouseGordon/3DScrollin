using NUnit.Framework;
using Player.Gravity;
using System;
using UnityEngine;

namespace Tests.EditorTests.Editor
{
    public class MockGravityData : IGravityData
    {
        public float GravityForce { get; set; } = -9.81f;
        public float TerminalVelocity { get; set; } = -20f;
        public float GroundedGravity { get; set; } = -2f;
        
        public static MockGravityData CreateDefault() => new()
        {
            GravityForce = -9.81f,
            TerminalVelocity = -20f,
            GroundedGravity = -2f
        };
    }
    
    [TestFixture]
    public class GravitySystemTests
    {
        private GravitySystem _gravitySystem;
        private MockGravityData _mockData;

        [SetUp]
        public void Setup()
        {
            _mockData = MockGravityData.CreateDefault();
            _gravitySystem = new GravitySystem(_mockData);
        }

        [Test]
        public void Constructor_WithNullData_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GravitySystem(null));
        }

        [Test]
        public void CalculateGravity_WhenGrounded_ReturnsGroundedGravity()
        {
            float gravity = _gravitySystem.CalculateGravity(true);
            Assert.That(gravity, Is.EqualTo(_mockData.GroundedGravity));
        }

        [Test]
        public void CalculateGravity_WhenInAir_AcceleratesDownward()
        {
            float initialVelocity = _gravitySystem.CalculateGravity(false);
            float laterVelocity = _gravitySystem.CalculateGravity(false);
            Assert.That(laterVelocity, Is.LessThan(initialVelocity));
        }

        [Test]
        public void CalculateGravity_WhenFalling_ClampsToTerminalVelocity()
        {
            // Simulate falling for a while
            for(int i = 0; i < 100; i++)
            {
                _gravitySystem.CalculateGravity(false);
            }

            float velocity = _gravitySystem.GetCurrentVerticalVelocity();
            Assert.That(velocity, Is.GreaterThanOrEqualTo(_mockData.TerminalVelocity));
        }

        [Test]
        public void CalculateGravity_TransitionFromGroundedToAirborne_ShowsGradualAcceleration()
        {
            // First grounded
            float groundedGravity = _gravitySystem.CalculateGravity(true);
            Assert.That(groundedGravity, Is.EqualTo(-2f));
    
            // Then airborne - first frame shows initial acceleration from grounded state
            float initialAirborneVelocity = _gravitySystem.CalculateGravity(false);
            Assert.That(initialAirborneVelocity, Is.EqualTo(-2.1962f).Within(0.0001f));
    
            // Next frame continues acceleration
            float nextFrameVelocity = _gravitySystem.CalculateGravity(false);
            Assert.That(nextFrameVelocity, Is.EqualTo(-2.3924f).Within(0.0001f));
    
            // Verify acceleration pattern
            Assert.That(nextFrameVelocity - initialAirborneVelocity, 
                Is.EqualTo(initialAirborneVelocity - groundedGravity).Within(0.0001f), 
                "Acceleration should be consistent between frames");
        }

        [Test]
        public void CalculateGravity_WhenGroundedAfterFalling_ResetsVelocity()
        {
            // First fall for a while
            for(int i = 0; i < 10; i++)
            {
                _gravitySystem.CalculateGravity(false);
            }

            // Then become grounded
            float groundedGravity = _gravitySystem.CalculateGravity(true);
            
            Assert.That(groundedGravity, Is.EqualTo(_mockData.GroundedGravity));
        }

        [Test]
        [Category("Performance")]
        public void CalculateGravity_With10000Iterations_CompletesQuickly()
        {
            var startTime = DateTime.Now;
            
            for(int i = 0; i < 10000; i++)
            {
                _gravitySystem.CalculateGravity(false);
            }
            
            var duration = (DateTime.Now - startTime).TotalMilliseconds;
            Assert.That(duration, Is.LessThan(100), "Gravity calculation took too long");
        }

        [Test]
        public void GetCurrentVerticalVelocity_InitialState_ReturnsZero()
        {
            float velocity = _gravitySystem.GetCurrentVerticalVelocity();
            Assert.That(velocity, Is.EqualTo(0f));
        }

        [Test]
        [TestCase(-30f)]
        [TestCase(-25f)]
        [TestCase(-15f)]
        public void CalculateGravity_WithDifferentTerminalVelocities_RespectsLimit(float terminalVelocity)
        {
            // Arrange
            _mockData.TerminalVelocity = terminalVelocity;
            _gravitySystem = new GravitySystem(_mockData);

            // Act - simulate falling for a while
            for(int i = 0; i < 200; i++)
            {
                _gravitySystem.CalculateGravity(false);
            }

            float finalVelocity = _gravitySystem.GetCurrentVerticalVelocity();
    
            // Assert
            Assert.That(finalVelocity, Is.GreaterThanOrEqualTo(terminalVelocity));
        }


        [Test]
        public void CalculateGravity_AlternatingGroundedState_MaintainsConsistency()
        {
            var expectedGroundedValue = _mockData.GroundedGravity;
            
            for(int i = 0; i < 10; i++)
            {
                float groundedGravity = _gravitySystem.CalculateGravity(true);
                Assert.That(groundedGravity, Is.EqualTo(expectedGroundedValue), 
                    $"Iteration {i}: Grounded gravity should remain constant");
                
                _gravitySystem.CalculateGravity(false); // One frame of falling
            }
        }

        [Test]
        public void CalculateGravity_WhenGroundedWithDifferentForces_UsesGroundedGravity()
        {
            // Arrange
            _mockData.GravityForce = -15f; // Stronger gravity
            _gravitySystem = new GravitySystem(_mockData);

            // Act
            float gravity = _gravitySystem.CalculateGravity(true);

            // Assert - should still use grounded gravity regardless of gravity force
            Assert.That(gravity, Is.EqualTo(_mockData.GroundedGravity));
        }
    }
}
