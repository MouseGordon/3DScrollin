using System;
using System.Collections.Generic;
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
            _movementSystem.SetMoveDirection(Vector2.right);
            
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
       
        [Test]
        [Category("Memory")]
        public void MovementSystem_UnderLoad_MaintainsStableMemory()
        {
            var initialMemory = GC.GetTotalMemory(true);
    
            // Simulate heavy load
            //
            for (int i = 0; i < 10000; i++)
            {
                _movementSystem.SetMoveDirection(new Vector2(
                    UnityEngine.Random.Range(-1f, 1f),
                    UnityEngine.Random.Range(-1f, 1f)
                ).normalized);
                _movementSystem.Move(0);
            }
    
            var finalMemory = GC.GetTotalMemory(true);
            var memoryDelta = finalMemory - initialMemory;
    
            // Ensure no significant memory growth
            //
            Assert.That(memoryDelta, Is.LessThan(1024 * 1024)); // Less than 1MB growth
        }

        [Test]
        [Category("ReplaySystem")]
        public void MovementSequence_WhenReplayed_MatchesOriginal()
        {
            var movements = new List<Vector2>();
            var inputs = new List<Vector2>();
    
            // Record sequence
            for (int i = 0; i < 100; i++)
            {
                var input = new Vector2(
                    UnityEngine.Random.Range(-1f, 1f),
                    UnityEngine.Random.Range(-1f, 1f)
                ).normalized;
        
                inputs.Add(input);
                _movementSystem.SetMoveDirection(input);
                movements.Add((Vector2)_movementSystem.Move(0));
            }
    
            // Reset and replay
            _movementSystem = new MovementSystem(_movementData);
    
            for (int i = 0; i < inputs.Count; i++)
            {
                _movementSystem.SetMoveDirection(inputs[i]);
                var replayedMovement = (Vector2)_movementSystem.Move(0);
        
                // Using Vector2 custom comparison
                Assert.That(Vector2.Distance(replayedMovement, movements[i]), Is.LessThan(0.0001f), 
                    $"Movement replay failed at index {i}. Expected {movements[i]}, got {replayedMovement}");
            }
        }
        
        [Test]
        [Category("Determinism")]
        public void Movement_WithSameInputs_ProducesSameOutputs()
        {
            var inputs = new[]
            {
                Vector2.right,
                Vector2.one.normalized,
                Vector2.left
            };
    
            // First run
            var firstRunResults = new List<Vector2>();
            foreach (var input in inputs)
            {
                _movementSystem.SetMoveDirection(input);
                firstRunResults.Add(_movementSystem.Move(0));
            }
    
            // Reset
            _movementSystem = new MovementSystem(_movementData);
    
            // Second run
            var secondRunResults = new List<Vector2>();
            foreach (var input in inputs)
            {
                _movementSystem.SetMoveDirection(input);
                secondRunResults.Add(_movementSystem.Move(0));
            }
    
            // Compare results
            for (int i = 0; i < inputs.Length; i++)
            {
                Assert.That(firstRunResults[i], Is.EqualTo(secondRunResults[i]));
            }
        }

    }
}