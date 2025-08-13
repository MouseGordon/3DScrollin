using NUnit.Framework;
using Player.Jump;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tests.EditorTests.Editor
{
    [TestFixture]
    public class JumpSystemTests
    {
        private JumpSystem _jumpSystem;
        private MockJumpData _mockData;
        private InputAction.CallbackContext _jumpPressed;
        private InputAction.CallbackContext _jumpReleased;

        [SetUp]
        public void Setup()
        {
            _mockData = new MockJumpData
            {
                JumpForce = 10f,
                DoubleJumpForce = 8f,
                TerminalVelocity = -20f,
                ApexThreshold = 0.5f,
                EnableDoubleJump = true,
                GroundedGravity = -2f,
                Gravity = -9.81f,
            };
            
            _jumpSystem = new JumpSystem(_mockData,-9.81f,2f);
            
            // We only need to know if it's performed or canceled
            _jumpPressed = default;
            _jumpReleased = default;
        }

        [Test]
        public void InitialState_IsGrounded()
        {
            // First check initial state
            Assert.That(_jumpSystem.CurrentJumpState, Is.EqualTo(JumpSystem.JumpState.Grounded));
    
            // Calculate one physics update to apply grounded gravity
            _jumpSystem.CalculateJumpVelocity(true);
    
            // Now check the velocity with gravity applied
            Assert.That(_jumpSystem.CurrentJumpState, Is.EqualTo(JumpSystem.JumpState.Grounded));
        }


        [Test]
        public void WhenJumping_TransitionsThroughExpectedStates(){
            const int MaxIterations = 1000;
            int iterations = 0;


            // Start grounded
            Assert.That(_jumpSystem.CurrentJumpState, Is.EqualTo(JumpSystem.JumpState.Grounded));

            // Initiate jump
            _jumpSystem.CalculateJumpVelocity(isGrounded: true);
            _jumpSystem.StartJump(true);
            _jumpSystem.CalculateJumpVelocity(false);

            // Should be rising
            Assert.That(_jumpSystem.CurrentJumpState, Is.EqualTo(JumpSystem.JumpState.Rising));
            Assert.That(_jumpSystem.Velocity, Is.EqualTo(_mockData.JumpForce));

            // Simulate rising until apex
            Debug.Log("=== Rising Phase ===");
            while (_jumpSystem.CurrentJumpState == JumpSystem.JumpState.Rising){
                var previousVelocity = _jumpSystem.Velocity;
                _jumpSystem.CalculateJumpVelocity(false);
                iterations++;
                if (iterations > MaxIterations)
                    Assert.Fail("Test exceeded maximum iterations while Rising");
            }
            
            // Should reach apex
            Assert.That(_jumpSystem.CurrentJumpState, Is.EqualTo(JumpSystem.JumpState.Apex));

            // Reset iteration counter
            iterations = 0;

            // Continue until falling
            Debug.Log("=== Apex Phase ===");
            while (_jumpSystem.CurrentJumpState == JumpSystem.JumpState.Apex){
                var previousVelocity = _jumpSystem.Velocity;
                _jumpSystem.CalculateJumpVelocity(false);
                iterations++;
                if (iterations > MaxIterations)
                    Assert.Fail("Test exceeded maximum iterations while at Apex");
            }

            Assert.That(_jumpSystem.CurrentJumpState, Is.EqualTo(JumpSystem.JumpState.Falling));
            _jumpSystem.CalculateJumpVelocity(true);
            Assert.That(_jumpSystem.CurrentJumpState, Is.EqualTo(JumpSystem.JumpState.Landing));
            _jumpSystem.CalculateJumpVelocity(true);
            Assert.That(_jumpSystem.CurrentJumpState, Is.EqualTo(JumpSystem.JumpState.Grounded));
        }


        [Test]
        public void DoubleJump_WhenEnabled_WorksInAir()
        {
            // Setup a mock input action for jumping
            var jumpAction = new InputAction(type: InputActionType.Button);
            jumpAction.Enable();
    
            // Need to start with a grounded physics update
            _jumpSystem.CalculateJumpVelocity(true);
    
            // Initial jump with properly mocked input
            _jumpSystem.StartJump(true);
            _jumpSystem.CalculateJumpVelocity(false);
    
            Assert.That(_jumpSystem.CurrentJumpState, Is.EqualTo(JumpSystem.JumpState.Rising), "Should transition to rising after jump input");
    
            // Perform double jump
            _jumpSystem.StartJump(true);
            _jumpSystem.CalculateJumpVelocity(false);
    
            Assert.That(_jumpSystem.Velocity, Is.EqualTo(_mockData.DoubleJumpForce));
        }


        [Test]
        public void DoubleJump_WhenDisabled_CannotJumpInAir()
        {
            _mockData.EnableDoubleJump = false;
            
            // Initial jump
            _jumpSystem.HandleJumpInput(_jumpPressed);
            _jumpSystem.CalculateJumpVelocity(false);
            float initialVelocity = _jumpSystem.Velocity;
            
            // Try double jump
            _jumpSystem.HandleJumpInput(_jumpPressed);
            _jumpSystem.CalculateJumpVelocity(false);
            
            // Velocity should not change to double jump force
            Assert.That(_jumpSystem.Velocity, Is.Not.EqualTo(_mockData.DoubleJumpForce));
        }

        [Test]
        public void Falling_WhenHittingGround_TransitionsToLanding()
        {
            const int MaxIterations = 1000;
            int iterations = 0;

            // Get into falling state
            _jumpSystem.CalculateJumpVelocity(isGrounded: true);
            _jumpSystem.StartJump(true);
            _jumpSystem.CalculateJumpVelocity(isGrounded: false);
            
            while (_jumpSystem.CurrentJumpState != JumpSystem.JumpState.Falling)
            {
                _jumpSystem.CalculateJumpVelocity(false);
                iterations++;
                if (iterations > MaxIterations)
                    Assert.Fail($"Never reached Falling state. Stuck in {_jumpSystem.CurrentJumpState} state");
            }

            // Verify we're actually in falling state
            Assert.That(_jumpSystem.CurrentJumpState, Is.EqualTo(JumpSystem.JumpState.Falling), 
                "Should be in Falling state before hitting ground");

            // Hit ground
            _jumpSystem.CalculateJumpVelocity(true);
    
            Assert.That(_jumpSystem.CurrentJumpState, Is.EqualTo(JumpSystem.JumpState.Landing), 
                $"Should transition to Landing when hitting ground. Current state: {_jumpSystem.CurrentJumpState}");
        }



        [Test]
        public void Velocity_NeverExceedsTerminalVelocity()
        {
            const int MaxIterations = 1000;
            int iterations = 0;

            // Get into falling state and let gravity accumulate
            _jumpSystem.CalculateJumpVelocity(isGrounded: true);
            _jumpSystem.StartJump(true);
            _jumpSystem.CalculateJumpVelocity(isGrounded: false);
            while (_jumpSystem.CurrentJumpState != JumpSystem.JumpState.Falling)
            {
                _jumpSystem.CalculateJumpVelocity(false);
                iterations++;
                if (iterations > MaxIterations)
                    Assert.Fail("Test exceeded maximum iterations while trying to reach Falling state");
            }
    
            // Let it fall for a while
            for (int i = 0; i < 100; i++)
            {
                _jumpSystem.CalculateJumpVelocity(false);
                Assert.That(_jumpSystem.Velocity, Is.GreaterThanOrEqualTo(_mockData.TerminalVelocity));
            }
        }
        
        [Test]
        public void Jump_WhenPressed_TransitionsToRising()
        {
            // Arrange
            _jumpSystem.CalculateJumpVelocity(true);  // Start grounded
    
            // Act
            _jumpSystem.StartJump(true);  // New method that doesn't require CallbackContext
            _jumpSystem.CalculateJumpVelocity(false);
    
            // Assert
            Assert.That(_jumpSystem.CurrentJumpState, Is.EqualTo(JumpSystem.JumpState.Rising));
        }


    }

    public class MockJumpData : IJumpData
    {
        public float JumpForce { get; set; }
        public float DoubleJumpForce { get; set; }
        public float Gravity { get; set; }
        public float TerminalVelocity { get; set; }
        public float GroundedGravity { get; set; }
        public float ApexThreshold { get; set; }
        public bool EnableDoubleJump { get; set; }
    }
}

