using System;
using NUnit.Framework;
using Player.Movement;
using UnityEngine;

namespace Tests.EditorTests.Editor{
    [TestFixture]
    public class MovementSystemTests
    {
        public class TestMovementData : IMovementData
        {
            public float MovementSpeed { get; set; }
            public float SprintSpeed { get; set; }
            public float TiredSpeed { get; set; }
    
            public static TestMovementData CreateDefault()
            {
                return new TestMovementData()
                {
                    MovementSpeed = 5f,
                    SprintSpeed = 10f,
                    TiredSpeed = 2f
                };
            }
        }

        private MovementSystem _movementSystem;
        private IMovementData _movementData;

        [SetUp]
        public void Setup()
        {
            _movementData =  new TestMovementData() {
                MovementSpeed = 5f,
                SprintSpeed = 10f,
                TiredSpeed = 2f
            };

        
            _movementSystem = new MovementSystem(_movementData);
        }

        [Test]
        public void Constructor_WithNullPlayerData_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MovementSystem(null));
        }

        [Test]
        public void IsSprinting_WhenNotSprintingAndMoving_ReturnsFalse()
        {
            _movementSystem.SetMoveDirection(Vector2.right);
            Assert.That(_movementSystem.IsSprinting, Is.False);
        }

        [Test]
        public void IsSprinting_WhenSprintingButNotMoving_ReturnsFalse()
        {
            _movementSystem.SetMoveDirection(Vector2.zero);
            _movementSystem.ToggleSprint();
            Assert.That(_movementSystem.IsSprinting, Is.False);
        }

        [Test]
        public void IsSprinting_WhenSprintingAndMoving_ReturnsTrue()
        {
            _movementSystem.SetMoveDirection(Vector2.right);
            _movementSystem.ToggleSprint();
            Assert.That(_movementSystem.IsSprinting, Is.True);
        }

        [Test]
        public void ToggleSprint_WhenToggledTwice_ReturnToNormalSpeed()
        {
            // First toggle to sprint
            _movementSystem.ToggleSprint();
            var movement1 = _movementSystem.Move(0);
        
            // Second toggle back to normal
            _movementSystem.ToggleSprint();
            var movement2 = _movementSystem.Move(0);
        
            Assert.That(movement2.magnitude, Is.LessThan(movement1.magnitude));
        }

        [Test]
        public void Move_WhenExhausted_UsesTiredSpeed()
        {
            _movementSystem.SetMoveDirection(Vector2.right);
    
            var movement = _movementSystem.Move(0,exhausted: true);
            var expectedMagnitude = _movementData.TiredSpeed * Time.fixedDeltaTime;
    
            Assert.That(movement.magnitude, Is.EqualTo(expectedMagnitude).Within(0.001f));
        }
        

        [Test]
        public void Move_WhenMovingRight_ReturnsPositiveX()
        {
            _movementSystem.SetMoveDirection(Vector2.right);
            var movement = _movementSystem.Move(0);
            Assert.That(movement.x, Is.Positive);
        }

        [Test]
        public void Move_WhenMovingLeft_ReturnsNegativeX()
        {
            _movementSystem.SetMoveDirection(Vector2.left);
            var movement = _movementSystem.Move(0);
            Assert.That(movement.x, Is.Negative);
        }

        [Test]
        public void Move_SprintingVsNormalSpeed_SprintingIsFaster()
        {
            _movementSystem.SetMoveDirection(Vector2.right);
            var normalMovement = _movementSystem.Move(0);
        
            _movementSystem.ToggleSprint();
            var sprintMovement = _movementSystem.Move(0);
        
            Assert.That(sprintMovement.magnitude, Is.GreaterThan(normalMovement.magnitude));
        }
        [Test]
        public void Move_WhenNotMoving_ReturnsZeroMagnitude()
        {
            _movementSystem.SetMoveDirection(Vector2.zero);
            var movement = _movementSystem.Move(0);
            Assert.That(movement.magnitude, Is.EqualTo(0f));
        }

        [Test]
        public void Move_WhenExhaustedAndSprinting_UsesTiredSpeed()
        {
            _movementSystem.SetMoveDirection(Vector2.right);
            _movementSystem.ToggleSprint();
    
            var movement = _movementSystem.Move(0,exhausted: true);
            var expectedMagnitude = _movementData.TiredSpeed * Time.fixedDeltaTime;
    
            Assert.That(movement.magnitude, Is.EqualTo(expectedMagnitude).Within(0.001f));
        }

        [Test]
        public void ToggleSprint_WhenExhausted_SpeedStillUsesTiredSpeed()
        {
            _movementSystem.SetMoveDirection(Vector2.right);
            _movementSystem.ToggleSprint();
    
            var movement = _movementSystem.Move(0,exhausted: true);
            var normalMovement = _movementSystem.Move(0,exhausted: true);
    
            Assert.That(movement.magnitude, Is.EqualTo(normalMovement.magnitude));
        }
       
      
       
        //  check this test when jump is refactored
        //
        /*[TestCase(0f, Description = "No jump")]
        [TestCase(5f, Description = "Normal jump")]
        [TestCase(-2f, Description = "Downward movement")]
        public void Move_WithDifferentJumpVelocities_AppliesCorrectVerticalMovement(float jumpVelocity)
        {
            var movement = _movementSystem.Move(jumpVelocity);
            Assert.That(movement.y, Is.EqualTo(jumpVelocity * _movementData.MovementSpeed * Time.fixedDeltaTime)
                .Within(0.001f));
        }*/
        
        /*[Test]
      public void Move_DiagonalMovement_MaintainsProperMagnitude()
      {
          _movementSystem.SetMoveDirection(new Vector2(1f, 1f).normalized);
          var movement = _movementSystem.Move(0);
          var expectedMagnitude = _movementData.MovementSpeed * Time.fixedDeltaTime;

          Assert.That(movement.magnitude, Is.EqualTo(expectedMagnitude).Within(0.001f));
      }*/
        
         /*[Test]
        public void Move_WithJumpVelocity_AppliesCorrectVerticalMovement()
        {
            float jumpVelocity = 5f;
            var movement = _movementSystem.Move(jumpVelocity);
        
            Assert.That(movement.y, Is.EqualTo(jumpVelocity * _movementData.MovementSpeed * Time.fixedDeltaTime)
                .Within(0.001f));
        }*/

    }
}