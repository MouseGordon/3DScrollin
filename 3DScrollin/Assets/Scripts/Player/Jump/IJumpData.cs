namespace Player.Jump{
    public interface IJumpData
    {
        float JumpForce { get; }
        float DoubleJumpForce { get; }
        float TerminalVelocity { get; }
        float ApexThreshold { get; }
        bool EnableDoubleJump { get; }
    }

}