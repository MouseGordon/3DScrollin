using System;
using Follow;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditorTests.Editor{
     [TestFixture]
    public class FollowerTests
    {
        private class MockFollowConfig : IFollowConfiguration
        {
            public Vector3 Offset { get; set; }
            public float SmoothTime { get; set; }
            public Transform Target { get; set; } // Not used in Follower anymore
        }

        [Test]
        public void UpdatePosition_WithOffset_MovesTowardsOffsetPosition()
        {
            // Arrange
            var config = new MockFollowConfig 
            {
                Offset = Vector3.up * 2, // 2 units up
                SmoothTime = 0.1f
            };
            
            var follower = new Follower(config);
            var currentPos = Vector3.zero;
            var targetPos = Vector3.right * 5; // 5 units right

            // Act
            var newPosition = follower.UpdatePosition(currentPos, targetPos);

            // Assert
            var expectedTarget = targetPos + config.Offset;
            Assert.That(newPosition.y, Is.GreaterThan(currentPos.y), "Should move upward due to offset");
            Assert.That(newPosition.x, Is.GreaterThan(currentPos.x), "Should move right towards target");
            Assert.That(Vector3.Distance(newPosition, expectedTarget), Is.LessThan(Vector3.Distance(currentPos, expectedTarget)), 
                "Should be closer to target than starting position");
        }

        [Test]
        public void UpdatePosition_FromDistance_MovesGradually()
        {
            // Arrange
            var config = new MockFollowConfig 
            {
                Offset = Vector3.zero,
                SmoothTime = 0.1f
            };
            
            var follower = new Follower(config);
            var startPos = Vector3.zero;
            var targetPos = Vector3.right * 10; // 10 units right

            // Act
            var newPosition = follower.UpdatePosition(startPos, targetPos);

            // Assert
            Assert.That(newPosition.x, Is.GreaterThan(0), "Should move towards target");
            Assert.That(newPosition.x, Is.LessThan(10), "Should not reach target immediately");
        }

        [Test]
        public void Velocity_MultipleUpdates_ShowsSmoothApproach()
        {
            // Arrange
            var config = new MockFollowConfig 
            {
                Offset = Vector3.zero,
                SmoothTime = 0.1f
            };
    
            var follower = new Follower(config);
            var currentPos = Vector3.zero;
            var targetPos = Vector3.right * 10;

            // Act & Assert
            // First update - should start moving
            currentPos = follower.UpdatePosition(currentPos, targetPos);
            var firstPosition = currentPos;
            Assert.That(firstPosition.x, Is.GreaterThan(0), "Should start moving towards target");

            // Second update - should continue moving
            currentPos = follower.UpdatePosition(currentPos, targetPos);
            var secondPosition = currentPos;
            Assert.That(secondPosition.x, Is.GreaterThan(firstPosition.x), "Should continue moving towards target");

            // Third update - should be even closer to target
            currentPos = follower.UpdatePosition(currentPos, targetPos);
            var thirdPosition = currentPos;
    
            // Calculate distances to target
            var firstDistance = Mathf.Abs(targetPos.x - firstPosition.x);
            var secondDistance = Mathf.Abs(targetPos.x - secondPosition.x);
            var thirdDistance = Mathf.Abs(targetPos.x - thirdPosition.x);

            // Assert distances are decreasing
            Assert.That(secondDistance, Is.LessThan(firstDistance), "Second position should be closer to target than first");
            Assert.That(thirdDistance, Is.LessThan(secondDistance), "Third position should be closer to target than second");
        }

    }
}
